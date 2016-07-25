using DCL.RecordFilterService.Configuration.Elements;
using DCL.RecordFilterService.Domain.Abstract;
using System;

namespace DCL.CustomFilterService.Logic.Actions
{
    public static class ActionFactory
    {
        /// <summary>
        /// Get the correct subclass of the Action abstract class based on the actionConfiguration's Type property
        /// </summary>
        /// <param name="actionConfig">The configuration information used to specify what action is needed</param>
        /// <param name="outputRepo">The file output repository used to write an action's output file</param>
        public static Action GetAction(ActionElement actionConfig, IRecordWritableRepository outputRepo)
        {
            switch (actionConfig.Type)
            {
                case ActionType.group:
                    return new GroupAction(actionConfig, outputRepo);
                case ActionType.remove:
                    return new RemoveAction(actionConfig, outputRepo);
                default:
                    throw new NotImplementedException("A action of type '" + actionConfig.Type + "' was specified, but no class has been implemented for it.  Supported actions are group and remove.");
            }
        }
    }
}
