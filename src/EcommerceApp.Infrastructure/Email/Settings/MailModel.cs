namespace Infrastructure.Email.Settings
{
    public class MailModel<T>
    {
        public T Model { get; set; } = default!;
        public string? TemplatePath { get; set; }
        public string To { get; set; } = null!;
    }
}
