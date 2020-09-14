using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Linq;
using System.Collections;

namespace RiseOfTheUndeaf.EntityEvents
{
    // This class provides a mechanism for broadcasting events throughout the entity tree.
    // Given an event interface:
    //
    //    public interface IMyEvent : IEntityEvent
    //    {
    //        void Event(int value);
    //    }
    //
    // Calling `Entity.BroadcastEvent<IMyEvent>()` will return a new object, implementing
    // the `IMyEvent` interface, called a broadcasting object, whose type is dynamically created.
    //
    //    public class DynamicImplementationOfIMyEvent : IMyEvent
    //    {
    //        private Entity entity;
    //        public DynamicImplementationOfIMyEvent(Entity arg1) => entity = arg1;
    //        public void Event(int value)
    //        {
    //            foreach (var component in entity.Components)
    //                if (component is IMyEvent @event)
    //                    @event.Event(value);
    //            var parent = entity;
    //            foreach(var child in entity.GetChildren())
    //            {
    //                entity = child;
    //                this.Event(value);
    //            }
    //            entity = parent;
    //        }
    //    }

    public static class EntityEventExtension
    {
        /// <summary>
        /// Cache of &lt;interface, broadcaster&gt; types, because reflection and emit are slow.
        /// </summary>
        private static Dictionary<Type, Type> dynamicInstanceCache = new Dictionary<Type, Type>();

        // Only create one assembly with one module
        private static AssemblyBuilder dynamicAssebly;
        private static ModuleBuilder dynamicModule;

        /// <summary>
        /// Creates a broadcast object that calls all components that implement <typeparamref name="TEvent"/> in the <paramref name="entity"/> and its children.
        /// </summary>
        /// <typeparam name="TEvent">Event interface type</typeparam>
        /// <param name="entity">Entity that initiates broadcast</param>
        /// <returns>Broadcasting object for given <paramref name="entity"/></returns>
        /// <remarks>The interface <typeparamref name="TEvent"/> may only carry non-generic, void methods.</remarks>
        public static TEvent BroadcastEvent<TEvent>(this Entity entity) where TEvent : IEntityEvent
        {
            var interfaceType = typeof(TEvent);

            if (!interfaceType.IsInterface)
                throw new ArgumentException("The generic argument must be an interface.", nameof(TEvent));

            // use cache if possible
            if (dynamicInstanceCache.TryGetValue(interfaceType, out var instanceType))
            {
                return (TEvent)Activator.CreateInstance(instanceType, entity);
            }

            EnsureDynamicModule();

            var typeName = $"DynamicImplementationOf{interfaceType.Name}";
            var typeBuilder = dynamicModule.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class);
            
            EmitTypeMembers(interfaceType, typeBuilder);

            typeBuilder.AddInterfaceImplementation(interfaceType);
            var type = typeBuilder.CreateTypeInfo().AsType();

            dynamicInstanceCache.Add(interfaceType, type);
            return (TEvent)Activator.CreateInstance(type, entity);
        }

        /// <summary>
        /// Emits a field of <see cref="Entity"/>, a constructor (which sets the field) and interface methods for broadcasting.
        /// </summary>
        /// <param name="interfaceType">Type of interface to be implemented</param>
        /// <param name="typeBuilder">Type being constructed</param>
        private static void EmitTypeMembers(Type interfaceType, TypeBuilder typeBuilder)
        {
            var entityField = typeBuilder.DefineField("entity", typeof(Entity), FieldAttributes.Private);

            var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(Entity) });
            EmitConstructor(entityField, ctor);

            var methods = interfaceType.GetMethods();
            foreach (var method in methods)
            {
                BuildMethod(typeBuilder, method, entityField, interfaceType);
            }
        }

        /// <summary>
        /// Initializes <see cref="dynamicAssebly"/> and <see cref="dynamicModule"/>.
        /// </summary>
        private static void EnsureDynamicModule()
        {
            dynamicAssebly ??= AssemblyBuilder.DefineDynamicAssembly(
                            new AssemblyName("DynamicAssembly"),
                            AssemblyBuilderAccess.Run);

            dynamicModule ??= dynamicAssebly.DefineDynamicModule("DynamicModule");
        }

        /// <summary>
        /// Emits constructor IL that takes one parameter of type <see cref="Entity"/> and saves it in the <paramref name="entityField"/>.
        /// </summary>
        /// <param name="entityField"></param>
        /// <param name="ctor"></param>
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

        /// <summary>
        /// Creates a broadcasting method, implementation of <paramref name="imethod"/>, on the new type.
        /// </summary>
        /// <param name="typeBuilder">Type under construction</param>
        /// <param name="imethod">Method to be implemented</param>
        /// <param name="entityField">Private field for the current <see cref="Entity"/></param>
        /// <param name="interfaceType">Interface to be implemented</param>
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

            var componentEnumerator = il.DeclareLocal(typeof(IEnumerator<EntityComponent>));
            var entityEnumerator = il.DeclareLocal(typeof(IEnumerator<Entity>));
            var parentEntity = il.DeclareLocal(typeof(Entity));

            // foreach (var component in entity.Components)
            //    if (component is IInterface @event)
            //       @event.Event(params...);
            {
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
            }

            // var parent = entity;
            // foreach(var child in entity.GetChildren())
            // {
            //     entity = child;
            //     this.Event(params...);
            // }
            // entity = parent;
            {
                il.Emit(OpCodes.Dup); //stack: entity, entity
                il.Emit(OpCodes.Stloc, parentEntity); //stack: entity

                il.EmitCall(OpCodes.Call, typeof(EntityExtensions).GetMethod("GetChildren", BindingFlags.Static | BindingFlags.Public), null); //stack: children
                il.EmitCall(OpCodes.Callvirt, typeof(IEnumerable<Entity>).GetMethod("GetEnumerator"), null); //stack: enumerator
                il.Emit(OpCodes.Stloc, entityEnumerator); //stack:

                var loop2start = il.DefineLabel();
                var loop2end = il.DefineLabel();

                il.MarkLabel(loop2start);

                il.Emit(OpCodes.Ldarg_0); //stack: this

                // if(!entityEnumerator.MoveNext) jump to end
                il.Emit(OpCodes.Ldloc, entityEnumerator); //stack: this, enumerator
                il.Emit(OpCodes.Dup); //stack: this, enumerator, enumerator
                il.EmitCall(OpCodes.Callvirt, typeof(IEnumerator).GetMethod("MoveNext"), null); //stack: this, enumerator, bool

                il.Emit(OpCodes.Brfalse_S, loop2end); //stack: this, enumerator

                il.EmitCall(OpCodes.Callvirt, typeof(IEnumerator).GetProperty("Current").GetGetMethod(), null); //stack: this, entity

                il.Emit(OpCodes.Stfld, entityField); //stack:
                il.Emit(OpCodes.Ldarg_0); //stack: this

                // call this.Method(params...) with this.enity = child
                for (int i = 1; i <= imethod.GetParameters().Length; i++)
                {
                    il.Emit(OpCodes.Ldarg, i);
                }
                il.EmitCall(OpCodes.Callvirt, method, null); //stack:

                il.Emit(OpCodes.Br, loop2start);

                il.MarkLabel(loop2end);
                il.Emit(OpCodes.Pop); //stack: this

                il.Emit(OpCodes.Ldloc, parentEntity); //stack: this, entity
                il.Emit(OpCodes.Stfld, entityField); //stack:
            }

            il.Emit(OpCodes.Ret);
        }
    }
}
