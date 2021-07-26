
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum CompanyType
    {
        [UiIgnoreEnumValue]

        [StringEnumDisplayText( "Public Company (Ltd)" )]
        Public = 0,

        [StringEnumDisplayText( "Private Company (Pty Ltd)" )]
        Private = 1,

        [StringEnumDisplayText( "Personal Liability Company (Inc)" )]
        Personal = 2,

        [StringEnumDisplayText( "Partnership" )]
        Partnership = 3,

        [StringEnumDisplayText( "Business Trust" )]
        BusinessTrust = 4,

        [StringEnumDisplayText( "Sole Proprietorship" )]
        SoleProprietorship = 5,

        [StringEnumDisplayText( "External Company" )]
        ExternalCompany = 6,

        [StringEnumDisplayText( "External Company (Foreign Country)" )]
        ForeignCompany = 7,

        [StringEnumDisplayText( "NPO (NGO)" )]
        NPO = 8,

        [StringEnumDisplayText( "Closed Corporation (CC)" )]
        ClosedCorporation = 9,
    }
}
