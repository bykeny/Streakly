namespace HabitGoalTrackerApp.Models
{
    /// <summary>
    /// Configuration settings for SMTP email sending.
    /// When all required properties are configured, real emails will be sent.
    /// If not configured, the app falls back to console logging (development mode).
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// SMTP server hostname (e.g., smtp.gmail.com, smtp.outlook.com)
        /// </summary>
        public string? SmtpHost { get; set; }

        /// <summary>
        /// SMTP server port (typically 587 for TLS, 465 for SSL, 25 for unencrypted)
        /// </summary>
        public int SmtpPort { get; set; } = 587;

        /// <summary>
        /// SMTP authentication username (usually your email address)
        /// </summary>
        public string? SmtpUsername { get; set; }

        /// <summary>
        /// SMTP authentication password or app-specific password
        /// </summary>
        public string? SmtpPassword { get; set; }

        /// <summary>
        /// Email address that appears in the "From" field
        /// </summary>
        public string? FromEmail { get; set; }

        /// <summary>
        /// Display name that appears in the "From" field (e.g., "Streakly")
        /// </summary>
        public string FromName { get; set; } = "Streakly";

        /// <summary>
        /// Whether to use SSL/TLS encryption (recommended: true)
        /// </summary>
        public bool UseSsl { get; set; } = true;

        /// <summary>
        /// Whether SMTP authentication is required (set to false for local testing with Mailpit)
        /// </summary>
        public bool RequireAuth { get; set; } = true;

        /// <summary>
        /// Checks if minimum required SMTP settings are configured for email sending.
        /// Host, Port, and FromEmail are always required.
        /// Username/Password are required only if RequireAuth is true.
        /// </summary>
        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(SmtpHost) &&
            !string.IsNullOrWhiteSpace(FromEmail) &&
            SmtpPort > 0;

        /// <summary>
        /// Checks if authentication credentials are provided.
        /// </summary>
        public bool HasCredentials =>
            !string.IsNullOrWhiteSpace(SmtpUsername) &&
            !string.IsNullOrWhiteSpace(SmtpPassword);
    }
}
