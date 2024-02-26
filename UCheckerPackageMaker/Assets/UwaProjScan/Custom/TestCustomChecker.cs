namespace UwaProjScan.ScanRule.CustomRules
{
    public class TestCustomChecker:ICustomRule
    {
        public bool Run(out bool hasTable, out RuleDataTable table)
        {
            hasTable = false;
            table = new RuleDataTable("才1", "Clo2", "Col3", "Col4", "Col5", "Col6");
            return true;
        }

        public string Description { get; }=  "一个描述";
        public ushort Id { get; } = 1001;
        public RulePriority Priority { get; } = RulePriority.High;
    }
}