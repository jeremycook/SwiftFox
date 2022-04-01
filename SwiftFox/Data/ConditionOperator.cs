using System.ComponentModel.DataAnnotations;

namespace SwiftFox.Data
{
    public enum ConditionOperator
    {
        Between,
        Contains,
        [Display(Name = "=")]
        EqualTo,
        [Display(Name = ">")]
        GreaterThan,
        [Display(Name = ">=")]
        GreaterThanOrEqualTo,
        [Display(Name = "<")]
        LessThan,
        [Display(Name = "<=")]
        LessThanOrEqualTo,
        [Display(Name = "Does Not Contain")]
        NotContains,
        [Display(Name = "!=")]
        NotEqualTo,
    }
}