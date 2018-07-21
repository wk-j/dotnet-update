namespace DotNetUpdate {
    public class Package {
        public string Id { set; get; }
        public string Version { set; get; }
        public override string ToString() => $"{Id} {Version}";
    }
}