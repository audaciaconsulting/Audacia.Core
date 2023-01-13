using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Audacia.Core.Tests.EnumMember
{
    public class EnumMemberTests
    {
        enum Users
        {
            Zahra = 1,
            Betahn = 2,
            Rai = 3,
            Rose = 4,
            Sam = 5,
            Fiona = 6,
            Faezan = 7
        }

        [Fact]
        public static void Check_if_Parsing_returns_expected_result()
        {
            //Arrange 
            var arrtibutes = new IEnumerable<CustomAttributeData> { DisplayName };
            var user = new MemberInfo() { CustomAttributes = }

            //Act

            //Assert

        }
    }
}