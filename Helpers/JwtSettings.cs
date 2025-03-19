namespace EventManagementSystem.Helpers
{
    /// <summary>
    /// Represents the settings used for configuring JWT (JSON Web Token) authentication.
    /// This class contains properties that define how the JWT tokens are generated, validated, and configured.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Gets or sets the name of the JWT token.
        /// This property defines the name used to reference the token in HTTP requests.
        /// </summary>
        public string JwtTokenName { get; set; }

        /// <summary>
        /// Gets or sets the secret key used for signing the JWT.
        /// This key is essential for ensuring the integrity and authenticity of the token.
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Gets or sets the issuer of the JWT.
        /// This property specifies the entity that issues the token, which can be used for validation.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the issuer of the token.
        /// If true, the token's issuer will be validated against the configured issuer.
        /// </summary>
        public bool ValidateIssuer { get; set; }

        /// <summary>
        /// Gets or sets the audience for the JWT.
        /// This property defines the intended recipient(s) of the token.
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the audience of the token.
        /// If true, the token's audience will be validated against the configured audience.
        /// </summary>
        public bool ValidateAudience { get; set; }

        /// <summary>
        /// Gets or sets the expiration time of the token in minutes.
        /// This property determines how long the token remains valid before it expires.
        /// </summary>
        public int TokenExpirationInMinutes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the lifetime of the token.
        /// If true, the token's expiration will be checked to ensure it is still valid.
        /// </summary>
        public bool ValidateLifetime { get; set; }
    }
}
