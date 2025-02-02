namespace UserSystem.Services.Models.Output
{
    public class LookupsOutputDto<T>
    {
        public T Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
    }
}
