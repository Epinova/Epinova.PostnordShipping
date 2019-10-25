namespace Epinova.PostnordShipping
{
    internal class FaultDto
    {
        public string ExplanationText { get; set; }
        public string FaultCode { get; set; }
        public ParamValueDto[] ParamValues { get; set; }
    }
}
