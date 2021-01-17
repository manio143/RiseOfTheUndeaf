using RiseOfTheUndeaf.GameEvents;
using RiseOfTheUndeaf.GameEvents.Events.Character;
using Stride.Engine;
using Stride.Engine.Processors;
using Stride.Games;

namespace RiseOfTheUndeaf.Character
{
    public class CharacterAttackProcessor : EntityProcessor<CharacterAttackComponent>
    {
        private GameEventSystem gameEventSystem;
        private ScriptSystem scriptSystem;

        protected override void OnSystemAdd()
        {
            gameEventSystem = GameEventSystem.GetFromServices(Services);
            scriptSystem = Services.GetService<ScriptSystem>();
        }

        public override void Update(GameTime time)
        {
            foreach (var kvp in ComponentDatas)
            {
                var component = kvp.Key;

                if (!component.CanExecute)
                    continue;

                if (component.State == CharacterAttackComponent.AttackState.Primary)
                {
                    if (component.PrimaryHandler != null)
                    {
                        scriptSystem.AddTask(async () => await component.PrimaryHandler.ExecuteAttack(
                            component.Entity,
                            (bool canExecute) => component.CanExecute = canExecute));
                    }

                    gameEventSystem.Log(new PrimaryAttackEvent(component.Entity));
                }
                else if(component.State == CharacterAttackComponent.AttackState.Secondary)
                {
                    if (component.SecondaryHandler != null)
                    {
                        scriptSystem.AddTask(async () => await component.SecondaryHandler.ExecuteAttack(
                            component.Entity,
                            (bool canExecute) => component.CanExecute = canExecute));
                    }

                    gameEventSystem.Log(new SecondaryAttackEvent(component.Entity));
                }

                component.State = CharacterAttackComponent.AttackState.None;
            }
        }
    }
}
