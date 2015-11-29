using Newtonsoft.Json;

namespace NRest.ModelBinders
{
    public class JsonModelBinder<T> : IModelBinder<T>
    {
        private readonly string json;

        public JsonModelBinder(string json)
        {
            this.json = json;
        }

        public JsonSerializerSettings Settings { get; set; }

        public void Bind(T instance)
        {
            JsonConvert.PopulateObject(json, instance, Settings ?? new JsonSerializerSettings());
        }
    }
}
