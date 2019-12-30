namespace ODrive.Sharp.Application
{
    public class GoogleUserInfo
    {
        //
        // Resumo:
        //     The user's email address.
        public string Email { get; set; }
        //
        // Resumo:
        //     The user's last name.
        public string FamilyName { get; set; }
        //
        // Resumo:
        //     The user's gender.
        public string Gender { get; set; }
        //
        // Resumo:
        //     The user's first name.
        public string GivenName { get; set; }
        //
        // Resumo:
        //     The hosted domain e.g. example.com if the user is Google apps user.
        public string Hd { get; set; }
        //
        // Resumo:
        //     The obfuscated ID of the user.
        public string Id { get; set; }
        //
        // Resumo:
        //     URL of the profile page.
        public string Link { get; set; }
        //
        // Resumo:
        //     The user's preferred locale.
        public string Locale { get; set; }
        //
        // Resumo:
        //     The user's full name.
        public string Name { get; set; }
        //
        // Resumo:
        //     URL of the user's picture image.
        public string Picture { get; set; }
        //
        // Resumo:
        //     Boolean flag which is true if the email address is verified. Always verified
        //     because we only return the user's primary email address.
        public bool? VerifiedEmail { get; set; }
        //
        // Resumo:
        //     The ETag of the item.
        public string ETag { get; set; }
    }
}