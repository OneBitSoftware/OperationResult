namespace OneBitSoftware.Utilities.OperationResult
{
    public class Error
    {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;

        public override string ToString()
        {
            if (this.Code != 0) return $"{this.Code}: {this.Message}";
            return this.Message;
        }
    }
}
