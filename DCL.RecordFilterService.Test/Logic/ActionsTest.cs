using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DCL.RecordFilterService.Configuration.Elements;
using DCL.RecordFilterService.Test.Fakes;
using DCL.CustomFilterService.Logic.Actions;
using DCL.RecordFilterService.Domain.Entities;

namespace DCL.RecordFilterService.Test.Logic
{
    /// <summary>
    /// Summary description for ActionsTest
    /// </summary>
    [TestClass]
    public class ActionsTest
    {
        #region General Action Tests ------------------------------------------
        /// <summary>
        /// Test that Action.DoesActionApplyToInput() behaves properly
        /// </summary>
        [TestMethod]
        public void Action_DoesActionApplyToInput()
        {
            // Arrange
            ActionElement groupActionElementForEveryone = new ActionElement() { Type = ActionType.group, GroupName = "TestGroup" };
            groupActionElementForEveryone.Elements.Add(new ConditionElement() { Type = ConditionType.allInclusive, Field = "FirstName" });
            FakeRecordWritableRepository fakeRepo0 = new FakeRecordWritableRepository();
            GroupAction groupActionForEveryone = new GroupAction(groupActionElementForEveryone, fakeRepo0);

            ActionElement groupActionElementWCust = new ActionElement() { Type = ActionType.group, GroupName = "TestGroup", Customer = "Tester" };
            groupActionElementWCust.Elements.Add(new ConditionElement() { Type = ConditionType.allInclusive, Field = "FirstName" });
            FakeRecordWritableRepository fakeRepo1 = new FakeRecordWritableRepository();
            GroupAction groupActionWithCustomer = new GroupAction(groupActionElementWCust, fakeRepo1);

            ActionElement groupActionElementWRecType = new ActionElement() { Type = ActionType.group, GroupName = "TestGroup", InputRecordType = "TestData" };
            groupActionElementWRecType.Elements.Add(new ConditionElement() { Type = ConditionType.allInclusive, Field = "FirstName" });
            FakeRecordWritableRepository fakeRepo2 = new FakeRecordWritableRepository();
            GroupAction groupActionWithRecType = new GroupAction(groupActionElementWRecType, fakeRepo2);

            ActionElement groupActionElementWCaRT = new ActionElement() { Type = ActionType.group, GroupName = "TestGroup", Customer = "Tester", InputRecordType = "TestData" };
            groupActionElementWCaRT.Elements.Add(new ConditionElement() { Type = ConditionType.allInclusive, Field = "FirstName" });
            FakeRecordWritableRepository fakeRepo3 = new FakeRecordWritableRepository();
            GroupAction groupActionWithCustomerAndRecType = new GroupAction(groupActionElementWCaRT, fakeRepo3);

            // Act
            bool doesActionForEveryoneApplyToAllInput = groupActionForEveryone.DoesActionApplyToInput("", "");
            bool doesActionForEveryoneApplyToCustomer = groupActionForEveryone.DoesActionApplyToInput("Tester", "");
            bool doesActionForEveryoneApplyToRecType = groupActionForEveryone.DoesActionApplyToInput("", "TestData");
            bool doesActionForEveryoneApplyToCustomerAndRecType = groupActionForEveryone.DoesActionApplyToInput("Tester", "TestData");

            bool doesActionWCustApplyToAllInput = groupActionWithCustomer.DoesActionApplyToInput("", "");
            bool doesActionWCustApplyToCustomer = groupActionWithCustomer.DoesActionApplyToInput("Tester", "");
            bool doesActionWCustApplyToRecType = groupActionWithCustomer.DoesActionApplyToInput("", "TestData");
            bool doesActionWCustApplyToCustomerAndRecType = groupActionWithCustomer.DoesActionApplyToInput("Tester", "TestData");

            bool doesActionWRecTypeApplyToAllInput = groupActionWithRecType.DoesActionApplyToInput("", "");
            bool doesActionWRecTypeApplyToCustomer = groupActionWithRecType.DoesActionApplyToInput("Tester", "");
            bool doesActionWRecTypeApplyToRecType = groupActionWithRecType.DoesActionApplyToInput("", "TestData");
            bool doesActionWRecTypeApplyToCustomerAndRecType = groupActionWithRecType.DoesActionApplyToInput("Tester", "TestData");

            bool doesActionWCaRTApplyToAllInput = groupActionWithCustomerAndRecType.DoesActionApplyToInput("", "");
            bool doesActionWCaRTApplyToCustomer = groupActionWithCustomerAndRecType.DoesActionApplyToInput("Tester", "");
            bool doesActionWCaRTApplyToRecType = groupActionWithCustomerAndRecType.DoesActionApplyToInput("", "TestData");
            bool doesActionWCaRTApplyToCustomerAndRecType = groupActionWithCustomerAndRecType.DoesActionApplyToInput("Tester", "TestData");
            bool doesActionWCaRTApplyToDifCustomer = groupActionWithCustomerAndRecType.DoesActionApplyToInput("NotATester", "");
            bool doesActionWCaRTApplyToDifRecType = groupActionWithCustomerAndRecType.DoesActionApplyToInput("", "NotTestData");
            bool doesActionWCaRTApplyToDifCustomerAndRecType = groupActionWithCustomerAndRecType.DoesActionApplyToInput("NotATester", "NotTestData");


            // Assert
            Assert.IsTrue(doesActionForEveryoneApplyToAllInput, "doesActionForEveryoneApplyToAllInput was false");
            Assert.IsTrue(doesActionForEveryoneApplyToCustomer, "doesActionForEveryoneApplyToCustomer was false");
            Assert.IsTrue(doesActionForEveryoneApplyToRecType, "doesActionForEveryoneApplyToRecType was false");
            Assert.IsTrue(doesActionForEveryoneApplyToCustomerAndRecType, "doesActionForEveryoneApplyToCustomerAndRecType was false");

            Assert.IsFalse(doesActionWCustApplyToAllInput, "doesActionWCustApplyToAllInput was true");
            Assert.IsTrue(doesActionWCustApplyToCustomer, "doesActionWCustApplyToCustomer was false");
            Assert.IsFalse(doesActionWCustApplyToRecType, "doesActionWCustApplyToRecType was true");
            Assert.IsTrue(doesActionWCustApplyToCustomerAndRecType, "doesActionWCustApplyToCustomerAndRecType was false");

            Assert.IsFalse(doesActionWRecTypeApplyToAllInput, "doesActionWRecTypeApplyToAllInput was true");
            Assert.IsFalse(doesActionWRecTypeApplyToCustomer, "doesActionWRecTypeApplyToCustomer was true");
            Assert.IsTrue(doesActionWRecTypeApplyToRecType, "doesActionWRecTypeApplyToRecType was false");
            Assert.IsTrue(doesActionWRecTypeApplyToCustomerAndRecType, "doesActionWRecTypeApplyToCustomerAndRecType was false");

            Assert.IsFalse(doesActionWCaRTApplyToAllInput, "doesActionWCaRTApplyToAllInput was true");
            Assert.IsFalse(doesActionWCaRTApplyToCustomer, "doesActionWCaRTApplyToCustomer was true");
            Assert.IsFalse(doesActionWCaRTApplyToRecType, "doesActionWCaRTApplyToRecType was true");
            Assert.IsTrue(doesActionWCaRTApplyToCustomerAndRecType, "doesActionWCaRTApplyToCustomerAndRecType was false");
            Assert.IsFalse(doesActionWCaRTApplyToDifCustomer, "doesActionWCaRTApplyToDifCustomer was true");
            Assert.IsFalse(doesActionWCaRTApplyToDifRecType, "doesActionWCaRTApplyToDifRecType was true");
            Assert.IsFalse(doesActionWCaRTApplyToDifCustomerAndRecType, "doesActionWCaRTApplyToDifCustomerAndRecType was true");
        }
        #endregion General Action Tests ---------------------------------------

        #region Group Action Tests --------------------------------------------
        /// <summary>
        /// Test that GroupAction.ChangeOutput(), .AreConditionsMet(), and .ProcessAction() behave appropriately
        /// </summary>
        [TestMethod]
        public void GroupAction()
        {
            // Arrange
            ActionElement groupActionElement = new ActionElement() { Type = ActionType.group, GroupName = "TestGroup" };
            groupActionElement.Elements.Add(new ConditionElement() { Type = ConditionType.allInclusive, Field = "FirstName" });
            FakeRecordWritableRepository fakeRepo = new FakeRecordWritableRepository();
            GroupAction simpleGroupAction = new GroupAction(groupActionElement, fakeRepo);
            Record person1 = new Record("FirstName,LastName,Age,Gender,PhoneNumber", "John,Doe,31,M,1112223333");
            Record person2 = new Record("FirstName,LastName,Age,Gender,PhoneNumber", "Jane,Li,20,F,4761234812");

            // Act
            simpleGroupAction.ChangeOutput("file1.cvs");
            Boolean isOutputChanged = (fakeRepo.OutputLocation.Equals("file1.cvs"));

            Boolean isAllInclusiveConditionMet = simpleGroupAction.AreConditionsMet(person1);

            simpleGroupAction.ProcessAction(person1);
            simpleGroupAction.ProcessAction(person2);
            Boolean wereCorrectPeopleWrittenToOutput = fakeRepo.Records.Contains(person1) && fakeRepo.Records.Contains(person2);

            // Assert
            Assert.IsTrue(isOutputChanged, "GroupAction.ChangeOutput('file1.cvs') did not update fakeRepo.OutputPath appropriately. The latter should have retuned 'file1.cvs', but it returned '" + fakeRepo.OutputLocation + "'.");
            Assert.IsTrue(isAllInclusiveConditionMet, "GroupAction.AreConditionsMet(person1) returned false when using an AllInclusiveCondition.  It should have returned true.");
            Assert.IsTrue(wereCorrectPeopleWrittenToOutput, "GroupAction.ProcessAction() did not write the correct Person objects to output.  It was supposed to write both person1 and person2, but the output only contains " + fakeRepo.Records.Count + " Person objects.");
        }

        /// <summary>
        /// Test that GroupAction.ProcessAction() behave appropriately with multiple conditions
        /// </summary>
        [TestMethod]
        public void GroupAction_MultipleConditions()
        {
            // Arrange
            ActionElement groupActionElement = new ActionElement() { Type = ActionType.group, GroupName = "TestGroup" };
            groupActionElement.Elements.Add(new ConditionElement() { Type = ConditionType.isAllowed, Field = "FirstName", Value="John" });
            groupActionElement.Elements.Add(new ConditionElement() { Type = ConditionType.isInRange, Field = "Age", RangeStart = 18, RangeEnd = 100 });
            FakeRecordWritableRepository fakeRepo = new FakeRecordWritableRepository();
            GroupAction complexGroupAction = new GroupAction(groupActionElement, fakeRepo);
            Record person1 = new Record("FirstName,LastName,Age,Gender,PhoneNumber", "John,Doe,15,M,1112223333");
            Record person2 = new Record("FirstName,LastName,Age,Gender,PhoneNumber", "Jane,Li,20,F,4761234812");
            Record person3 = new Record("FirstName,LastName,Age,Gender,PhoneNumber", "John,Li,25,M,2225553465");

            // Act
            complexGroupAction.ProcessAction(person1);
            complexGroupAction.ProcessAction(person2);
            complexGroupAction.ProcessAction(person3);
            Boolean wasCorrectPersonWrittenToOutput = fakeRepo.Records.Count == 1 && fakeRepo.Records.Contains(person3);

            // Assert
            Assert.IsTrue(wasCorrectPersonWrittenToOutput, "GroupAction.ProcessAction() did not write the correct Person object to output.  It was supposed to write person3, but the output contains " + fakeRepo.Records.Count + " Person objects.");
        }
        #endregion Group Action Tests -----------------------------------------

        #region Remove Action Tests -------------------------------------------
        /// <summary>
        /// Test that RemoveAction.ChangeOutput(), .AreConditionsMet(), and .ProcessAction() behave appropriately
        /// </summary>
        [TestMethod]
        public void RemoveAction()
        {
            // Arrange
            ActionElement removeActionElement = new ActionElement() { Type = ActionType.remove, GroupName = "TestRemove" };
            removeActionElement.Elements.Add(new ConditionElement() { Type = ConditionType.allInclusive, Field = "FirstName" });
            FakeRecordWritableRepository fakeRepo = new FakeRecordWritableRepository();
            RemoveAction simpleRemoveAction = new RemoveAction(removeActionElement, fakeRepo);
            Record person1 = new Record("FirstName,LastName,Age,Gender,PhoneNumber", "John,Doe,31,M,1112223333");
            Record person2 = new Record("FirstName,LastName,Age,Gender,PhoneNumber", "Jane,Li,20,F,4761234812");

            // Act
            simpleRemoveAction.ChangeOutput("file2.cvs");
            Boolean isOutputChanged = (fakeRepo.OutputLocation.Equals("file2.cvs"));

            Boolean isAllInclusiveConditionMet = simpleRemoveAction.AreConditionsMet(person1);

            simpleRemoveAction.ProcessAction(person1);
            simpleRemoveAction.ProcessAction(person2);
            Boolean wereCorrectPeopleWrittenToOutput = fakeRepo.Records.Count == 0;

            // Assert
            Assert.IsTrue(isOutputChanged, "RemoveAction.ChangeOutput('file2.cvs') did not update fakeRepo.OutputPath appropriately. The latter should have retuned 'file2.cvs', but it returned '" + fakeRepo.OutputLocation + "'.");
            Assert.IsTrue(isAllInclusiveConditionMet, "RemoveAction.AreConditionsMet(person1) returned false when using an AllInclusiveCondition.  It should have returned true.");
            Assert.IsTrue(wereCorrectPeopleWrittenToOutput, "RemoveAction.ProcessAction() did not exclude the correct Person objects from output.  It was supposed write to nothing, but the output contains " + fakeRepo.Records.Count + " Person objects.");
        }

        /// <summary>
        /// Test that RemoveAction.ProcessAction() behave appropriately with multiple conditions
        /// </summary>
        [TestMethod]
        public void RemoveAction_MultipleConditions()
        {
            // Arrange
            ActionElement removeActionElement = new ActionElement() { Type = ActionType.remove, GroupName = "TestRemove" };
            removeActionElement.Elements.Add(new ConditionElement() { Type = ConditionType.isAllowed, Field = "FirstName", Value = "John" });
            removeActionElement.Elements.Add(new ConditionElement() { Type = ConditionType.isInRange, Field = "Age", RangeStart = 18, RangeEnd = 100 });
            FakeRecordWritableRepository fakeRepo = new FakeRecordWritableRepository();
            RemoveAction complexGroupAction = new RemoveAction(removeActionElement, fakeRepo);
            Record person1 = new Record("FirstName,LastName,Age,Gender,PhoneNumber", "John,Doe,15,M,1112223333");
            Record person2 = new Record("FirstName,LastName,Age,Gender,PhoneNumber", "Jane,Li,20,F,4761234812");
            Record person3 = new Record("FirstName,LastName,Age,Gender,PhoneNumber", "John,Li,25,M,2225553465");

            // Act
            complexGroupAction.ProcessAction(person1);
            complexGroupAction.ProcessAction(person2);
            complexGroupAction.ProcessAction(person3);
            Boolean wereCorrectPeopleWrittenToOutput = fakeRepo.Records.Count == 2 && fakeRepo.Records.Contains(person1) && fakeRepo.Records.Contains(person2);

            // Assert
            Assert.IsTrue(wereCorrectPeopleWrittenToOutput, "RemoveAction.ProcessAction() did not write the correct Person objects to output.  It was supposed to write person1 and person2, but the output contains " + fakeRepo.Records.Count + " Person objects.");
        }
        #endregion Remove Action Tests ----------------------------------------

        #region Action Factory Tests ------------------------------------------
        /// <summary>
        /// Test if ActionFactory.GetAction returns the correct implementation of the 
        ///  abstract Action class matching the configuration info passed it
        /// </summary>
        [TestMethod]
        public void ActionFactory_GetAction()
        {
            // Arrange
            ActionElement groupActionConfig = new ActionElement() { Type = ActionType.group, GroupName = "TestGroup" };
            ActionElement removeActionConfig = new ActionElement() { Type = ActionType.remove };
            FakeRecordWritableRepository fakeRepo = new FakeRecordWritableRepository();

            // Act
            CustomFilterService.Logic.Actions.Action isGroupAction = ActionFactory.GetAction(groupActionConfig, fakeRepo);
            CustomFilterService.Logic.Actions.Action isRemoveAction = ActionFactory.GetAction(removeActionConfig, fakeRepo);

            // Assert
            Assert.IsInstanceOfType(isGroupAction, typeof(GroupAction), "ActionFactory.GetAction(groupActionConfig) returned an object that is not of type " + typeof(GroupAction).ToString());
            Assert.IsInstanceOfType(isRemoveAction, typeof(RemoveAction), "ActionFactory.GetAction(removeActionConfig)  returned an object that is not of type " + typeof(RemoveAction).ToString());
        }
        #endregion Action Factory Tests ---------------------------------------
    }
}
