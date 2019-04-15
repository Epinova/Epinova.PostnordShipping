using System.Collections.Generic;

namespace Epinova.PostnordShipping
{
    internal class ServicePointInformationComparer : IEqualityComparer<ServicePointInformation>
    {
        public bool Equals(ServicePointInformation x, ServicePointInformation y)
        {
            if (y == null && x == null)
                return true;
            if (x == null || y == null)
                return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(ServicePointInformation obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}