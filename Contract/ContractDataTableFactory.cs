using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contract
{
    public static class ContractDataTableFactory
    {
        internal const string ContractCompanyTableName = "contract_company";
        internal const string GeneralCompanyTableName = "general";
        internal const string RateTableName = "rates";
        internal const string RuleDetailTableName = "rule_detail";
        internal const string RuleTableName = "rules";

        public static ContractDataTable Create(ContractBusObject busObject, string tableName)
        {
            switch (tableName)
            {
                case ContractCompanyTableName:
                    return new ContractCompanyDataTable(busObject, tableName);
                case GeneralCompanyTableName:
                    return new GeneralDataTable(busObject, tableName);
                case RateTableName:
                    return new RateDataTable(busObject, tableName);
                case RuleDetailTableName:
                    return new RuleDetailDataTable(busObject, tableName);
                case RuleTableName:
                    return new RuleDataTable(busObject, tableName);
                default:
                    return new ContractDataTable(busObject, tableName);
            }
        }
    }
}
