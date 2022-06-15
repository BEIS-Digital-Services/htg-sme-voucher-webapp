

namespace BEIS.HelpToGrow.Voucher.Web.Common
{
    public class NotDefaultAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "The {0} field must not have the default value";

        public NotDefaultAttribute() : base(DefaultErrorMessage) 
        { 
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            var type = value.GetType();

            if (!type.IsValueType)
            {
                return true;
            }

            var defaultValue = Activator.CreateInstance(type);
            
            return !value.Equals(defaultValue);
        }
    }
}