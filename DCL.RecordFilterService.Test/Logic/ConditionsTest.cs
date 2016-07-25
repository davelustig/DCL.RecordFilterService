using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DCL.CustomFilterService.Logic.Conditions;
using DCL.RecordFilterService.Domain.Entities;
using DCL.RecordFilterService.Configuration.Elements;

namespace DCL.RecordFilterService.Test
{
    [TestClass]
    public class ConditionsTest
    {
        #region AllowedCondition Tests ----------------------------------------
        /// <summary>
        /// Test if AllowedCondition.IsMet correctly matches on single or multiple values
        /// </summary>
        [TestMethod]
        public void AllowedCondition_IsMet_HandlesSingleAndMultipleValues()
        {
            // Arrange
            AllowedCondition allowMenCondition = new AllowedCondition("Gender", "M");
            AllowedCondition allowMenOrWomenCondition = new AllowedCondition("Gender", "M,F");
            Record man = new Record("Gender","M");
            Record woman = new Record("Gender", "F");
            Record unknown = new Record("Gender", "U");
            Record empty = new Record();

            // Act
            Boolean allowMenMatchesGenderM = allowMenCondition.IsMet(man);
            Boolean allowMenMatchesGenderF = allowMenCondition.IsMet(woman);
            Boolean allowMenMatchesGenderU = allowMenCondition.IsMet(unknown);
            Boolean allowMenMatchesNoGender = allowMenCondition.IsMet(empty);
            Boolean allowMFMatchesGenderM = allowMenOrWomenCondition.IsMet(man);
            Boolean allowMFMatchesGenderF = allowMenOrWomenCondition.IsMet(woman);
            Boolean allowMFMatchesGenderU = allowMenOrWomenCondition.IsMet(unknown);
            Boolean allowMFMatchesNoGender = allowMenOrWomenCondition.IsMet(empty);

            // Assert
            Assert.IsTrue(allowMenMatchesGenderM, "AllowedCondition(field = 'Gender', value = 'M') did not match Record('Gender','M')");
            Assert.IsFalse(allowMenMatchesGenderF, "AllowedCondition(field = 'Gender', value = 'M') wrongly matched Record('Gender','F')");
            Assert.IsFalse(allowMenMatchesGenderU, "AllowedCondition(field = 'Gender', value = 'M') wrongly matched Record('Gender','U')");
            Assert.IsFalse(allowMenMatchesNoGender, "AllowedCondition(field = 'Gender', value = 'M') wrongly matched Record('Gender','')");
            Assert.IsTrue(allowMFMatchesGenderM, "AllowedCondition(field = 'Gender', value = 'M,F') did not match Record('Gender','M')");
            Assert.IsTrue(allowMFMatchesGenderF, "AllowedCondition(field = 'Gender', value = 'M,F') did not match Record('Gender','F')");
            Assert.IsFalse(allowMFMatchesGenderU, "AllowedCondition(field = 'Gender', value = 'M,F') wrongly matched Record('Gender','U')");
            Assert.IsFalse(allowMFMatchesNoGender, "AllowedCondition(field = 'Gender', value = 'M,F') wrongly matched Record('Gender','')");
        }

        /// <summary>
        /// Test if AllowedCondition.IsMet correctly matches for each supported field/property of Person 
        /// </summary>
        [TestMethod]
        public void AllowedCondition_IsMet_HandlesAllFields()
        {
            // Arrange
            AllowedCondition allowFirstNameCondition = new AllowedCondition("FirstName", "Jane");
            AllowedCondition allowLastNameCondition = new AllowedCondition("LastName", "Doe");
            AllowedCondition allowPhoneNumberCondition = new AllowedCondition("PhoneNumber", "1112223333");
            AllowedCondition allowAgeCondition = new AllowedCondition("Age", "30");
            AllowedCondition allowGenderCondition = new AllowedCondition("Gender", "F");
            Record allowedPerson = new Record("FirstName,LastName,Age,Gender,PhoneNumber", "Jane,Doe,30,F,1112223333");
            Record deniedPerson = new Record("FirstName,LastName,Age,Gender,PhoneNumber", "Jon,Do,31,U,1112223344");
            Record emptyPerson = new Record();

            // Act
            Boolean firstNameAllowed = allowFirstNameCondition.IsMet(allowedPerson);
            Boolean lastNameAllowed = allowLastNameCondition.IsMet(allowedPerson);
            Boolean phoneNumberAllowed = allowPhoneNumberCondition.IsMet(allowedPerson);
            Boolean ageAllowed = allowAgeCondition.IsMet(allowedPerson);
            Boolean genderAllowed = allowGenderCondition.IsMet(allowedPerson);
            Boolean firstNameNotAllowed = allowFirstNameCondition.IsMet(deniedPerson);
            Boolean lastNameNotAllowed = allowLastNameCondition.IsMet(deniedPerson);
            Boolean phoneNumberNotAllowed = allowPhoneNumberCondition.IsMet(deniedPerson);
            Boolean ageNotAllowed = allowAgeCondition.IsMet(deniedPerson);
            Boolean genderNotAllowed = allowGenderCondition.IsMet(deniedPerson);
            Boolean emptyFirstNameNotAllowed = allowFirstNameCondition.IsMet(emptyPerson);
            Boolean emptyLastNameNotAllowed = allowLastNameCondition.IsMet(emptyPerson);
            Boolean emptyPhoneNumberNotAllowed = allowPhoneNumberCondition.IsMet(emptyPerson);
            Boolean emptyAgeNotAllowed = allowAgeCondition.IsMet(emptyPerson);
            Boolean emptyGenderNotAllowed = allowGenderCondition.IsMet(emptyPerson);

            // Assert
            Assert.IsTrue(firstNameAllowed, "AllowedCondition(field = 'FirstName', value = 'Jane') did not match Record('FirstName,LastName,Age,Gender,PhoneNumber', 'Jane,Doe,30,F,1112223333')");
            Assert.IsTrue(lastNameAllowed, "AllowedCondition(field = 'LastName', value = 'Doe') did not match Record('FirstName,LastName,Age,Gender,PhoneNumber', 'Jane,Doe,30,F,1112223333')");
            Assert.IsTrue(phoneNumberAllowed, "AllowedCondition(field = 'PhoneNumber', value = '1112223333') did not match Record('FirstName,LastName,Age,Gender,PhoneNumber', 'Jane,Doe,30,F,1112223333')");
            Assert.IsTrue(ageAllowed, "AllowedCondition(field = 'Age', value = '30') did not match Record('FirstName,LastName,Age,Gender,PhoneNumber', 'Jane,Doe,30,F,1112223333')");
            Assert.IsTrue(genderAllowed, "AllowedCondition(field = 'Gender', value = 'F') did not match Record('FirstName,LastName,Age,Gender,PhoneNumber', 'Jane,Doe,30,F,1112223333')");

            Assert.IsFalse(firstNameNotAllowed, "AllowedCondition(field = 'FirstName', value = 'Jane') wrongly matched Record('FirstName,LastName,Age,Gender,PhoneNumber', 'Jon,Do,31,U,1112223344')");
            Assert.IsFalse(lastNameNotAllowed, "AllowedCondition(field = 'LastName', value = 'Doe') wrongly matched Record('FirstName,LastName,Age,Gender,PhoneNumber', 'Jon,Do,31,U,1112223344')");
            Assert.IsFalse(phoneNumberNotAllowed, "AllowedCondition(field = 'PhoneNumber', value = '1112223333') wrongly matched Record('FirstName,LastName,Age,Gender,PhoneNumber', 'Jon,Do,31,U,1112223344')");
            Assert.IsFalse(ageNotAllowed, "AllowedCondition(field = 'Age', value = '30') wrongly matched Record('FirstName,LastName,Age,Gender,PhoneNumber', 'Jon,Do,31,U,1112223344')");
            Assert.IsFalse(genderNotAllowed, "AllowedCondition(field = 'Gender', value = 'F') wrongly matched Record('FirstName,LastName,Age,Gender,PhoneNumber', 'Jon,Do,31,U,1112223344')");

            Assert.IsFalse(emptyFirstNameNotAllowed, "AllowedCondition(field = 'FirstName', value = 'Jane') wrongly matched an empty Record()");
            Assert.IsFalse(emptyLastNameNotAllowed, "AllowedCondition(field = 'LastName', value = 'Doe') wrongly matched an empty Record()");
            Assert.IsFalse(emptyPhoneNumberNotAllowed, "AllowedCondition(field = 'PhoneNumber', value = '1112223333') wrongly matched an empty Record()");
            Assert.IsFalse(emptyAgeNotAllowed, "AllowedCondition(field = 'Age', value = '30') wrongly matched an empty Record()");
            Assert.IsFalse(emptyGenderNotAllowed, "AllowedCondition(field = 'Gender', value = 'F') wrongly matched an empty Record()");
        }
        #endregion AllowedCondition Tests -------------------------------------

        #region AllowedCondition Tests ----------------------------------------
        /// <summary>
        /// Test if RangeCondition.IsMet correctly matches values within range, and test border conditions
        /// </summary>
        [TestMethod]
        public void RangedCondition_IsMet_HandlesRange()
        {
            // Arrange
            RangedCondition isPersonAdultCondition = new RangedCondition("Age", 18, 100);
            // Arrange - for simple in and out-of-range tests
            Record adult = new Record("Age", "30");
            Record child = new Record("Age", "10");
            Record corpse = new Record("Age", "150");
            // Arrange - for boundary tests
            Record fetus = new Record("Age", "-1");
            Record almostAdult = new Record("Age", "17");
            Record newAdult = new Record("Age", "18");
            Record almostDead = new Record("Age", "100");
            Record rip = new Record("Age", "101");

            // Act
            Boolean isPersonAdultMatchesAdult = isPersonAdultCondition.IsMet(adult);
            Boolean isPersonAdultMatchesChild = isPersonAdultCondition.IsMet(child);
            Boolean isPersonAdultMatchesCorpse = isPersonAdultCondition.IsMet(corpse);
            Boolean isPersonAdultMatchesFetus = isPersonAdultCondition.IsMet(fetus);
            Boolean isPersonAdultMatchesAlmostAdult = isPersonAdultCondition.IsMet(almostAdult);
            Boolean isPersonAdultMatchesNewAdult = isPersonAdultCondition.IsMet(newAdult);
            Boolean isPersonAdultMatchesAlmostDead = isPersonAdultCondition.IsMet(almostDead);
            Boolean isPersonAdultMatchesRIP = isPersonAdultCondition.IsMet(rip);

            // Assert
            Assert.IsTrue(isPersonAdultMatchesAdult, "RangedCondition(field = 'Age', rangeStart = 18, rangeEnd = 100) did not match Record('Age', '30')");
            Assert.IsFalse(isPersonAdultMatchesChild, "RangedCondition(field = 'Age', rangeStart = 18, rangeEnd = 100) wrongly matched Record('Age', '10')");
            Assert.IsFalse(isPersonAdultMatchesCorpse, "RangedCondition(field = 'Age', rangeStart = 18, rangeEnd = 100) wrongly matched Record('Age', '150')");
            Assert.IsFalse(isPersonAdultMatchesFetus, "RangedCondition(field = 'Age', rangeStart = 18, rangeEnd = 100) wrongly matched Record('Age', '-1')");
            Assert.IsFalse(isPersonAdultMatchesAlmostAdult, "RangedCondition(field = 'Age', rangeStart = 18, rangeEnd = 100) wrongly matched Record('Age', '17')");
            Assert.IsTrue(isPersonAdultMatchesNewAdult, "RangedCondition(field = 'Age', rangeStart = 18, rangeEnd = 100) did not match Record('Age', '18')");
            Assert.IsTrue(isPersonAdultMatchesAlmostDead, "RangedCondition(field = 'Age', rangeStart = 18, rangeEnd = 100) did not match Record('Age', '100')");
            Assert.IsFalse(isPersonAdultMatchesRIP, "RangedCondition(field = 'Age', rangeStart = 18, rangeEnd = 100) wrongly matched Record('Age', '101')");
        }

        /// <summary>
        /// Test if RangedCondition.IsMet correctly matches for each supported field/property of Person
        /// </summary>
        [TestMethod]
        public void RangedCondition_IsMet_HandlesFields()
        {
            // Arrange
            RangedCondition isPersonAdultCondition = new RangedCondition("Age", 18, 100);
            RangedCondition interestingPhoneCondition = new RangedCondition("PhoneNumber", 1112223333, 1112223355);
            Record suspicious = new Record("FirstName,LastName,Age,Gender,PhoneNumber", "Jane,Doe,30,F,1112223333");
            Record boring = new Record("FirstName,LastName,Age,Gender,PhoneNumber", "Jon,Do,17,U,2221113344");

            // Act
            Boolean isPersonAdultMatchesSuspicious = isPersonAdultCondition.IsMet(suspicious);
            Boolean isPersonAdultMatchesBoring = isPersonAdultCondition.IsMet(boring);
            Boolean interestingPhoneMatchesSuspicious = interestingPhoneCondition.IsMet(suspicious);
            Boolean interestingPhoneMatchesBoring = interestingPhoneCondition.IsMet(boring);

            // Assert
            Assert.IsTrue(isPersonAdultMatchesSuspicious, "RangedCondition(field = 'Age', rangeStart = 18, rangeEnd = 100) did not match Record('FirstName,LastName,Age,Gender,PhoneNumber', 'Jane,Doe,30,F,1112223333')");
            Assert.IsFalse(isPersonAdultMatchesBoring, "RangedCondition(field = 'Age', rangeStart = 18, rangeEnd = 100) wrongly matched Record('FirstName,LastName,Age,Gender,PhoneNumber', 'Jon,Do,17,U,2221113344')");
            Assert.IsTrue(interestingPhoneMatchesSuspicious, "RangedCondition(field = 'PhoneNumber', rangeStart = 1112223333, rangeEnd = 1112223355) did not match Record('FirstName,LastName,Age,Gender,PhoneNumber', 'Jane,Doe,30,F,1112223333')");
            Assert.IsFalse(interestingPhoneMatchesBoring, "RangedCondition(field = 'PhoneNumber', rangeStart = 1112223333, rangeEnd = 1112223355) wrongly matched Record('FirstName,LastName,Age,Gender,PhoneNumber', 'Jon,Do,17,U,2221113344')");
        }
        #endregion AllowedCondition Tests -------------------------------------

        #region DuplicateCondition Tests --------------------------------------
        /// <summary>
        /// Test if DuplicateCondition.IsMet correctly returns false for initial entries and true for duplicates
        /// when configured to evaluate duplicates based on a single field
        /// </summary>
        [TestMethod]
        public void DuplicateCondition_IsMet_OnSingleField()
        {
            // Arrange
            DuplicateCondition duplicateGenderCondition = new DuplicateCondition("Gender");
            Record man1 = new Record("Gender", "M");
            Record woman1 = new Record("Gender", "F");
            Record man2 = new Record("Gender", "M");
            Record woman2 = new Record("Gender", "F");
            Record empty1 = new Record();
            Record empty2 = new Record();

            // Act
            Boolean isMan1Duplicate = duplicateGenderCondition.IsMet(man1);
            Boolean isWoman1Duplicate = duplicateGenderCondition.IsMet(woman1);
            Boolean isMan2Duplicate = duplicateGenderCondition.IsMet(man2);
            Boolean isWoman2Duplicate = duplicateGenderCondition.IsMet(woman2);
            Boolean isEmpty1Duplicate = duplicateGenderCondition.IsMet(empty1);
            Boolean isEmpty2Duplicate = duplicateGenderCondition.IsMet(empty2);

            // Assert
            Assert.IsFalse(isMan1Duplicate, "DuplicateCondition(field = 'Gender') wrongly matched the first instance of Record('Gender','M')");
            Assert.IsFalse(isWoman1Duplicate, "DuplicateCondition(field = 'Gender') wrongly matched the first instance of Record('Gender','F')");
            Assert.IsTrue(isMan2Duplicate, "DuplicateCondition(field = 'Gender') did not match the second instance of Record('Gender','M')");
            Assert.IsTrue(isWoman2Duplicate, "DuplicateCondition(field = 'Gender') did not match second instance of Record('Gender','F')");
            Assert.IsFalse(isEmpty1Duplicate, "DuplicateCondition(field = 'Gender') wrongly matched the first instance of an empty Record()");
            Assert.IsTrue(isEmpty2Duplicate, "DuplicateCondition(field = 'Gender') did not match second instance of an emtpy Record()");
        }

        /// <summary>
        /// Test if DuplicateCondition.IsMet correctly returns false for initial entries and true for duplicates
        /// when configured to evaluate duplicates based on multiple fields
        /// </summary>
        [TestMethod]
        public void DuplicateCondition_IsMet_OnMultipleFields()
        {
            // Arrange
            DuplicateCondition duplicateGenderCondition = new DuplicateCondition("LastName,Age,Gender");
            Record person1 = new Record("FirstName,LastName,Age,Gender", "John,Doe,30,M");
            Record person1NewLastName = new Record("FirstName,LastName,Age,Gender", "John,Li,30,M");
            Record person1NewAge = new Record("FirstName,LastName,Age,Gender", "John,Doe,31,M");
            Record person1NewGender = new Record("FirstName,LastName,Age,Gender", "John,Doe,30,F");
            Record person1NewLastNameAge = new Record("FirstName,LastName,Age,Gender", "John,Li,31,M");
            Record person1NewAgeGender = new Record("FirstName,LastName,Age,Gender", "John,Doe,31,F");
            Record person2 = new Record("FirstName,LastName,Age,Gender", "John,Doe,30,M");
            Record person2NewLastName = new Record("FirstName,LastName,Age,Gender", "John,Li,30,M");
            Record person2NewAge = new Record("FirstName,LastName,Age,Gender", "John,Doe,31,M");
            Record person2NewGender = new Record("FirstName,LastName,Age,Gender", "John,Doe,30,F");
            Record person2NewLastNameAge = new Record("FirstName,LastName,Age,Gender", "John,Li,31,M");
            Record person2NewAgeGender = new Record("FirstName,LastName,Age,Gender", "John,Doe,31,F");
            Record empty1 = new Record();
            Record empty2 = new Record();

            // Act
            Boolean isPerson1Duplicate = duplicateGenderCondition.IsMet(person1);
            Boolean isPerson1NewLastNameDuplicate = duplicateGenderCondition.IsMet(person1NewLastName);
            Boolean isPerson1NewAgeDuplicate = duplicateGenderCondition.IsMet(person1NewAge);
            Boolean isPerson1NewGenderDuplicate = duplicateGenderCondition.IsMet(person1NewGender);
            Boolean isPerson1NewLastNameAgeDuplicate = duplicateGenderCondition.IsMet(person1NewLastNameAge);
            Boolean isPerson1NewAgeGenderDuplicate = duplicateGenderCondition.IsMet(person1NewAgeGender);
            Boolean isPerson2Duplicate = duplicateGenderCondition.IsMet(person2);
            Boolean isPerson2NewLastNameDuplicate = duplicateGenderCondition.IsMet(person2NewLastName);
            Boolean isPerson2NewAgeDuplicate = duplicateGenderCondition.IsMet(person2NewAge);
            Boolean isPerson2NewGenderDuplicate = duplicateGenderCondition.IsMet(person2NewGender);
            Boolean isPerson2NewLastNameAgeDuplicate = duplicateGenderCondition.IsMet(person2NewLastNameAge);
            Boolean isPerson2NewAgeGenderDuplicate = duplicateGenderCondition.IsMet(person2NewAgeGender);
            Boolean isEmpty1Duplicate = duplicateGenderCondition.IsMet(empty1);
            Boolean isEmpty2Duplicate = duplicateGenderCondition.IsMet(empty2);

            // Assert
            Assert.IsFalse(isPerson1Duplicate, "DuplicateCondition(field = 'LastName,Age,Gender') wrongly matched the first instance of Record('FirstName,LastName,Age,Gender', 'John,Doe,30,M')");
            Assert.IsFalse(isPerson1NewLastNameDuplicate, "DuplicateCondition(field = 'LastName,Age,Gender') wrongly matched the first instance of Record('FirstName,LastName,Age,Gender', 'John,Li,30,M')");
            Assert.IsFalse(isPerson1NewAgeDuplicate, "DuplicateCondition(field = 'LastName,Age,Gender') wrongly matched the first instance of Record('FirstName,LastName,Age,Gender', 'John,Doe,31,M')");
            Assert.IsFalse(isPerson1NewGenderDuplicate, "DuplicateCondition(field = 'LastName,Age,Gender') wrongly matched the first instance of Record('FirstName,LastName,Age,Gender', 'John,Doe,30,F')");
            Assert.IsFalse(isPerson1NewLastNameAgeDuplicate, "DuplicateCondition(field = 'LastName,Age,Gender') wrongly matched the first instance of Record('FirstName,LastName,Age,Gender', 'John,Li,31,M')");
            Assert.IsFalse(isPerson1NewAgeGenderDuplicate, "DuplicateCondition(field = 'LastName,Age,Gender') wrongly matched the first instance of Record('FirstName,LastName,Age,Gender', 'John,Doe,31,F')");
            Assert.IsTrue(isPerson2Duplicate, "DuplicateCondition(field = 'LastName,Age,Gender') did not match the second instance of Record('FirstName,LastName,Age,Gender', 'John,Doe,30,M')");
            Assert.IsTrue(isPerson2NewLastNameDuplicate, "DuplicateCondition(field = 'LastName,Age,Gender') did not match the second instance of Record('FirstName,LastName,Age,Gender', 'John,Li,30,M')");
            Assert.IsTrue(isPerson2NewAgeDuplicate, "DuplicateCondition(field = 'LastName,Age,Gender') did not match the second instance of Record('FirstName,LastName,Age,Gender', 'John,Doe,31,M')");
            Assert.IsTrue(isPerson2NewGenderDuplicate, "DuplicateCondition(field = 'LastName,Age,Gender') did not match the second instance of Record('FirstName,LastName,Age,Gender', 'John,Doe,30,F')");
            Assert.IsTrue(isPerson2NewLastNameAgeDuplicate, "DuplicateCondition(field = 'LastName,Age,Gender') did not match the second instance of Record('FirstName,LastName,Age,Gender', 'John,Li,31,M')");
            Assert.IsTrue(isPerson2NewAgeGenderDuplicate, "DuplicateCondition(field = 'LastName,Age,Gender') did not match the second instance of Record('FirstName,LastName,Age,Gender', 'John,Doe,31,F')");
            Assert.IsFalse(isEmpty1Duplicate, "DuplicateCondition(field = 'LastName,Age,Gender') wrongly matched the first instance of an empty Record()");
            Assert.IsTrue(isEmpty2Duplicate, "DuplicateCondition(field = 'LastName,Age,Gender') did not match second instance of an empty Record()");
        }

        /// <summary>
        /// Test if DuplicateCondition.Reset correctly flushes all input history and 
        ///  DuplicateCondition.IsMet operates correctly before and after a Reset
        /// </summary>
        [TestMethod]
        public void DuplicateCondition_IsMet_ForMultipleInputs()
        {
            // Arrange
            DuplicateCondition duplicateGenderCondition = new DuplicateCondition("Gender");
            Record input1Man1 = new Record("Gender", "M");
            Record input1Woman1 = new Record("Gender", "F");
            Record input1Man2 = new Record("Gender", "M");
            Record input1Woman2 = new Record("Gender", "F");
            Record input2Man1 = new Record("Gender", "M");
            Record input2Woman1 = new Record("Gender", "F");

            // Act
            Boolean isInput1Man1Duplicate = duplicateGenderCondition.IsMet(input1Man1);
            Boolean isInput1Woman1Duplicate = duplicateGenderCondition.IsMet(input1Woman1);
            Boolean isInput1Man2Duplicate = duplicateGenderCondition.IsMet(input1Man2);
            Boolean isInput1Woman2Duplicate = duplicateGenderCondition.IsMet(input1Woman2);
            duplicateGenderCondition.Reset();
            Boolean isInput2Man1Duplicate = duplicateGenderCondition.IsMet(input2Man1);
            Boolean isInput2Woman1Duplicate = duplicateGenderCondition.IsMet(input2Woman1);

            // Assert
            Assert.IsFalse(isInput1Man1Duplicate, "DuplicateCondition(field = 'Gender') wrongly matched the first instance of Record('Gender','M') in input group 1");
            Assert.IsFalse(isInput1Woman1Duplicate, "DuplicateCondition(field = 'Gender') wrongly matched the first instance of Record('Gender','F') in input group 1");
            Assert.IsTrue(isInput1Man2Duplicate, "DuplicateCondition(field = 'Gender') did not match the second instance of Record('Gender','M') in input group 1");
            Assert.IsTrue(isInput1Woman2Duplicate, "DuplicateCondition(field = 'Gender') did not match second instance of Record('Gender','F') in input group 1");
            Assert.IsFalse(isInput2Man1Duplicate, "DuplicateCondition(field = 'Gender') wrongly matched the first instance of Record('Gender','M') in input group 2");
            Assert.IsFalse(isInput2Woman1Duplicate, "DuplicateCondition(field = 'Gender') wrongly matched the first instance of Record('Gender','F') in input group 2");
        }
        #endregion DuplicateCondition Tests -----------------------------------

        #region Condition Factory Tests ---------------------------------------
        /// <summary>
        /// Test if ConditionFactory.GetCondition returns the correct implementation of the 
        ///  ICondition interface matching the configuration info passed it
        /// </summary>
        [TestMethod]
        public void ConditionFactory_GetCondition()
        {
            // Arrange
            ConditionElement allowedConditionConfig = new ConditionElement() { Type = ConditionType.isAllowed, Field = "Gender", Value = "F" };
            ConditionElement duplicateConditionConfig = new ConditionElement() { Type = ConditionType.isDuplicate, Field = "Gender" };
            ConditionElement rangedConditionConfig = new ConditionElement() { Type = ConditionType.isInRange, Field = "Age", RangeStart = 18, RangeEnd = 100 };

            // Act
            ICondition isAllowedCondition = ConditionFactory.GetCondition(allowedConditionConfig);
            ICondition isDuplicateCondition = ConditionFactory.GetCondition(duplicateConditionConfig);
            ICondition isInRangeCondition = ConditionFactory.GetCondition(rangedConditionConfig);

            // Assert
            Assert.IsInstanceOfType(isAllowedCondition, typeof(AllowedCondition), "ConditionFactory.GetCondition(allowedConditionConfig) returned an object that is not of type " + typeof(AllowedCondition).ToString());
            Assert.IsInstanceOfType(isDuplicateCondition, typeof(DuplicateCondition), "ConditionFactory.GetCondition(duplicateConditionConfig) returned an object that is not of type " + typeof(DuplicateCondition).ToString());
            Assert.IsInstanceOfType(isInRangeCondition, typeof(RangedCondition), "ConditionFactory.GetCondition(rangedConditionConfig) returned an object that is not of type " + typeof(RangedCondition).ToString());
        }
        #endregion Condition Factory Tests ------------------------------------
    }
}

