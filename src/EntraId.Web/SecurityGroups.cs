using Microsoft.IdentityModel.Protocols.Configuration;

namespace EntraId.Web
{
    public class SecurityGroups : Dictionary<string, string>
    {
        public const string Development = "Development";
        public const string ProductOwner = "Product Owner";

        public SecurityGroups(IConfiguration configuration)
        {
            configuration.Bind("SecurityGroups", this);

            if (!this.Any())
                throw new InvalidConfigurationException("SecurityGroups configuration undefined!");
        }
    }
}
