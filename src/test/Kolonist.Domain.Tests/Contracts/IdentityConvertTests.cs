using Kolonist.Contracts.Identities;
using Machine.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolonist.Domain.Tests.Contracts
{
    [Subject(typeof(IdentityConvert))]
    public abstract class IdentityConvertTests
    {
        protected static Guid Id;
        protected static IIdentity Identity;

        Establish context = () => Id = Guid.NewGuid();
    }

    public class Can_Convert_BuildingId_From_Transportable : IdentityConvertTests
    {
        static string Transportable;


        Establish context = () => Transportable = "Building-" + Id.ToString();
        Because of = () => Identity = IdentityConvert.FromTransportable(Transportable);

        It should_not_be_null = () => Identity.ShouldNotBeNull();
        It should_be_of_type_BuildingId = () => Identity.ShouldBeOfType<BuildingId>();
        It should_have_the_correct_id = () => ((BuildingId)Identity).Id.ShouldEqual(Id);
    }

    public class Can_Convert_BuildingTypeId_From_Transportable : IdentityConvertTests
    {
        static string Transportable;

        Establish context = () => Transportable = "BuildingType-" + Id.ToString();
        Because of = () => Identity = IdentityConvert.FromTransportable(Transportable);

        It should_not_be_null = () => Identity.ShouldNotBeNull();
        It should_be_of_type_BuildingTypeId = () => Identity.ShouldBeOfType<BuildingTypeId>();
    }
}
