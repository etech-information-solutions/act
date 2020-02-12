
namespace ACT.Core.Enums
{
    using Attributes;

    [StringEnum]
    public enum DinnerClaimRejectionReasons
    {
        [UiIgnoreEnumValue]
        [StringEnumDisplayText( "All" )]
        All = 0,

        [StringEnumDisplayText( "No authorisation code requested prior to dining" )]
        NoAuthorisationCode = 1, 

        [StringEnumDisplayText( "Non-participating restaurant" )]
        NonParticipatingRestaurant = 2,

        [StringEnumDisplayText( "Specials not permitted" )]
        SpecialsNotPermitted = 3,

        [StringEnumDisplayText( "2 Drink requirement not adhered to" )]
        TwoDrinkRequirement = 4,

        [StringEnumDisplayText( "2 Meal requirement not adhered to" )]
        TwoMealRequirement = 5,

        [StringEnumDisplayText( "Dessert/top-up's not permitted" )]
        DessertNotPermitted = 6,

        [StringEnumDisplayText( "Discount received from the restaurant" )]
        RestaurantDiscounted = 7,

        [StringEnumDisplayText( "Take-away not permitted" )]
        NoTakeAway = 8,

        [StringEnumDisplayText( "Loyalty card not permitted" )]
        LoyaltyCardNotPermitted = 9,

        [StringEnumDisplayText( "Claim submitted after 1 calender month" )]
        LateClaim = 10,

        [StringEnumDisplayText( "Invalid restaurant reciept (Faded, EFT only, hand written) " )]
        InvalidReceipt = 11,

        [StringEnumDisplayText( "Multiple usage on the same day at the same restaurant not permitted" )]
        MultipleUsage = 12,

        [StringEnumDisplayText( "Redeeming multiple benefits at the same time not permitted" )]
        multipleRedeeming = 13,

        [StringEnumDisplayText( "Claiming twice on the same reciept not permitted" )]
        ClaimingTwice = 14,

        [StringEnumDisplayText( "Autorisation code requested after dining not permitted" )]
        LateAutorisationCodeRequest = 15,

        [StringEnumDisplayText( "Authorisation code requested for different restaurant" )]
        InvalidAutorisationCodeRequest = 16,

        [StringEnumDisplayText( "Promotional vouchers not permitted" )]
        PromotionalVouchers = 17,

        [StringEnumDisplayText( "Membership not active" )]
        MembershipNotActive = 18,
    }
}
