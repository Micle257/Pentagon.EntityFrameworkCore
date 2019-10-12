namespace Pentagon.EntityFrameworkCore {
    using System.Reflection;

    public class ConflictPairDifference
    {
        public string PropertyName { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public object LocalValue { get; set; }

        public object RemoteValue { get; set; }
    }
}