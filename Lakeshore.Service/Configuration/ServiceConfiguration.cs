namespace Lakeshore.Microservice.Configurations
{
    public class VendorSettings
    {
        public string VendorName { get; set; }
    }

    public class ServiceConfiguration
    {
        public VendorSettings vendorSettings { get; set; }
        public ValidationSettings validationSettings { get; set; }

    }

    public class ValidationSettings
    {
        public string InBoundSchema { get; set; }
    }

}
