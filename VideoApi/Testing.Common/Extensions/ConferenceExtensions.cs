using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace Testing.Common.Extensions
{
    public static class ConferenceExtensions
    {
        public static void SetSupplier(this Conference conference, Supplier supplier)
        {
            conference.SetProtectedProperty(nameof(conference.Supplier), supplier);
        }
    }
}
