namespace PetStore.Common
{
    public static class GlobalConstants
    {
        //Breed
        public const int BreedNameMinLength = 5;
        public const int BreedNameMaxLength = 30;

        //Client
        public const int UserNameMinLength = 6;
        public const int UserNameMaxLength = 30;

        public const int EmailNameMinLength = 5;
        public const int EmailNameMaxLength = 50;

        public const int ClientFirstNameMinLength = 5;
        public const int ClientFirstNameMaxLength = 30;

        public const int ClientLastNameMinLength = 5;
        public const int ClientLastNameMaxLength = 30;
        //ClientProduct

        public const int QuantityMinValue = 1;

        //Order
        public const int TownMinLength = 3;
        public const int TownMaxLength = 15;

        public const int AddressMinLength = 5;
        public const int AddressMaxLength = 30;

        //Pet
        public const int PetNameMinLength = 3;
        public const int PetNameMaxLength = 30;
        public const int PetMinPrice = 0;
        public const int PetMinAge = 0;
        public const int PetMaxAge = 200;

        //Product
        public const int ProductNameMinLength = 3;
        public const int ProductNameMaxLength = 3;
        public const int ProductMinPrice = 0;
    }
}
