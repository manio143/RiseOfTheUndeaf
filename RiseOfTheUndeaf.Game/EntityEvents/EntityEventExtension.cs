using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Linq;
using System.Collections;

namespace RiseOfTheUndeaf.EntityEvents
{
    public static class EntityEventExtension
    {
        private static Dictionary<Type, Type> dynamicInstanceCache = new Dictionary<Type, Type>();
        private static AssemblyBuilder dynamicAssebly;
        private static ModuleBuilder dynamicModule;

        /// <summary>
        /// Creates a broadcast object that calls all components that implement <typeparamref name="TEvent"/> in the <paramref name="entity"/> and its children.
        /// </summary>
        /// <typeparam name="TEvent">Event interface type</typeparam>
        /// <param name="entity">Entity that initiates broadcast</param>
        /// <returns>Broadcasting object for given <paramref name="entity"/></returns>
        public static TEvent BroadcastEvent<TEvent>(this Entity entity) where TEvent : IEntityEvent
        {
            var interfaceType = typeof(TEvent);

            // use cache if possible
            if (dynamicInstanceCache.TryGetValue(interfaceType, out var instanceType))
            {
                return (TEvent)Activator.CreateInstance(instanceType, entity);
            }

            dynamicAssebly ??= AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("DynamicAssembly"),
                AssemblyBuilderAccess.Run);

            dynamicModule ??= dynamicAssebly.DefineDynamicModule("DynamicModule");

            var typeName = $"DynamicImplementationOf{interfaceType.Name}";
            var typeBuilder = dynamicModule.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class);

            var entityField = typeBuilder.DefineField("entity", typeof(Entity), FieldAttributes.Private);

            var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(Entity) });
            EmitConstructor(entityField, ctor);

            var methods = interfaceType.GetMethods();
            foreach (var method in methods)
            {
                BuildMethod(typeBuilder, method, entityField, interfaceType);
            }

            typeBuilder.AddInterfaceImplementation(interfaceType);
            var type = typeBuilder.CreateTypeInfo().AsType();

            dynamicInstanceCache.Add(interfaceType, type);
            return (TEvent)Activator.CreateInstance(type, entity);
        }

        private static void EmitConstructor(FieldBuilder entityField, ConstructorBuilder ctor)
        {
            var ctorIL = ctor.GetILGenerator();

            Type objType = Type.GetType("System.Object");
            ConstructorInfo objCtor = objType.GetConstructor(new Type[0]);

            // invoke superclass constructor
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Call, objCtor);

            // save parameter into the field
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_1);
            ctorIL.Emit(OpCodes.Stfld, entityField);

            ctorIL.Emit(OpCodes.Ret);
        }

        private static void BuildMethod(TypeBuilder typeBuilder, MethodInfo imethod, FieldBuilder entityField, Type interfaceType)
        {
            if (imethod.ReturnType != typeof(void))
                throw new ArgumentException($"Method '{imethod.Name}' has a non-void return type '{imethod.ReturnType.Name}'");

            var method = typeBuilder.DefineMethod(
                imethod.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                imethod.ReturnType,
                imethod.GetParameters().Select(p => p.ParameterType).ToArray()
            );
            var il = method.GetILGenerator();

            // create a new object x of type * : TEvent
            // such that x.SomeMethod(params) {
            //    foreach(var component in entity.Components)
            //        if(component is TEvent) component.SomeMethod(params)
            //    foreach(var child in entity.GetChildren)
            //        child.BroadcastEvent<TEvent>().SomeMethod(params);
            // }

            var componentEnumerator = il.DeclareLocal(typeof(IEnumerator<EntityComponent>));
            var entityEnumerator = il.DeclareLocal(typeof(IEnumerator<Entity>));

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, entityField); //stack: entity
            il.Emit(OpCodes.Dup); //stack: entity, entity

            il.EmitCall(OpCodes.Callvirt, typeof(Entity).GetProperty("Components").GetGetMethod(), null); //stack: entity, components
            il.EmitCall(OpCodes.Callvirt, typeof(IEnumerable<EntityComponent>).GetMethod("GetEnumerator"), null); //stack: entity, enumerator
            il.Emit(OpCodes.Stloc, componentEnumerator); //stack: entity

            var loop1start = il.DefineLabel();
            var loop1end = il.DefineLabel();

            il.MarkLabel(loop1start);

            // if(!componentEnumerator.MoveNext) jump to end
            il.Emit(OpCodes.Ldloc, componentEnumerator); //stack: entity, enumerator
            il.Emit(OpCodes.Dup); //stack: entity, enumerator, enumerator
            il.EmitCall(OpCodes.Callvirt, typeof(IEnumerator).GetMethod("MoveNext"), null); //stack: entity, enumerator, bool

            il.Emit(OpCodes.Brfalse_S, loop1end); //stack: entity, enumerator

            // else{if(enumerator.Current is TEvent) component.SomeMethod(params)
            il.EmitCall(OpCodes.Callvirt, typeof(IEnumerator).GetProperty("Current").GetGetMethod(), null); //stack: entity, component
            il.Emit(OpCodes.Isinst, interfaceType); //stack: entity, component/null
            il.Emit(OpCodes.Dup); //stack: entity, component/null, component/null

            var if1 = il.DefineLabel();
            var if1_ = il.DefineLabel();
            il.Emit(OpCodes.Brfalse_S, if1); //stack: entity, component/null
            for (int i = 1; i <= imethod.GetParameters().Length; i++)
            {
                il.Emit(OpCodes.Ldarg, i);
            }
            il.EmitCall(OpCodes.Callvirt, imethod, null); //stack: entity

            il.Emit(OpCodes.Br_S, if1_);

            il.MarkLabel(if1);
            il.Emit(OpCodes.Pop); //stack: entity

            il.MarkLabel(if1_);
            il.Emit(OpCodes.Br, loop1start);

            il.MarkLabel(loop1end);
            il.Emit(OpCodes.Pop); //stack: entity

            il.EmitCall(OpCodes.Call, typeof(EntityExtensions).GetMethod("GetChildren", BindingFlags.Static | BindingFlags.Public), null); //stack: children
            il.EmitCall(OpCodes.Callvirt, typeof(IEnumerable<Entity>).GetMethod("GetEnumerator"), null); //stack: enumerator
            il.Emit(OpCodes.Stloc, entityEnumerator); //stack:

            var loop2start = il.DefineLabel();
            var loop2end = il.DefineLabel();

            il.MarkLabel(loop2start);

            // if(!entityEnumerator.MoveNext) jump to end
            il.Emit(OpCodes.Ldloc, entityEnumerator); //stack: enumerator
            il.Emit(OpCodes.Dup); //stack: enumerator, enumerator
            il.EmitCall(OpCodes.Callvirt, typeof(IEnumerator).GetMethod("MoveNext"), null); //stack: enumerator, bool

            il.Emit(OpCodes.Brfalse_S, loop2end); //stack: enumerator

            // child.BroadcastEvent<TEvent>().SomeMethod(params);)
            il.EmitCall(OpCodes.Callvirt, typeof(IEnumerator).GetProperty("Current").GetGetMethod(), null); //stack: entity

            il.EmitCall(OpCodes.Call, typeof(EntityEventExtension).GetMethod(nameof(BroadcastEvent), BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(interfaceType), null); //stack: interface
            for (int i = 1; i <= imethod.GetParameters().Length; i++)
            {
                il.Emit(OpCodes.Ldarg, i);
            }
            il.EmitCall(OpCodes.Callvirt, imethod, null); //stack:

            il.Emit(OpCodes.Br, loop2start);

            il.MarkLabel(loop2end);
            il.Emit(OpCodes.Pop); //stack:

            il.Emit(OpCodes.Ret);
        }
    }
}
