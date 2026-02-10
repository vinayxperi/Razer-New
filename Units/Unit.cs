using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RazerBase;
using System.Data;

namespace Units
{
    public class Unit : NotifyingObject
    {
        [Required]
        public int contract_id { get; set; }

        public int cs_id { get; set; }
        
        [StringLength(30)]
        public string cs_name { get; set; }

        public string psa_city { get; set; }

        public string psa_state { get; set; }

        [Required]
        [StringLength(30)]
        public string mso_name { get; set; }

        [Required]
        [StringLength(8)]
        public string product_code { get; set; }

        [Required]
        public DateTime service_period_start { get; set; }

        [Required]
        public DateTime service_period_end { get; set; }
        
        [Range(typeof(decimal), "-99999999999999.99", "99999999999999.99")]
        public decimal amount { get; set; }

        [Required]
        [StringLength(80)]
        public string contract_description { get; set; }

        [StringLength(40)]
        public string report_description { get; set; }

        [Required]
        [StringLength(40)]
        public string unit_description { get; set; }

        [Required]
        [StringLength(8)]
        public string unit_period_type_description { get; set; }

        //begin metadata sections
        [StringLength(80)]
        public string destination_country { get; set; }

        [StringLength(80)]
        public string manufactured_country { get; set; }

        [StringLength(40)]
        public string model { get; set; }

        [StringLength(20)]
        public string technology { get; set; }

        [StringLength(25)]
        public string manufactured_product { get; set; }

        [StringLength(40)]
        public string brand { get; set; }

        [StringLength(60)]
        public string subscriber { get; set; }

        [StringLength(60)]
        public string ancillary { get; set; }

        [StringLength(40)]
        public string data { get; set; }

        [StringLength(80)]
        public string replicator { get; set; }

        [StringLength(255)]
        public string title { get; set; }

        [StringLength(40)]
        public string software { get; set; }

        [StringLength(30)]
        public string oem { get; set; }

        [StringLength(60)]
        public string tivo_count_description { get; set; }


        public int report_id { get; set; }

        public int estimated_flag { get; set; }

        //DWR-Added 5/7/12 - Boolean which signifies if record has any metadata associated with it - Defaults to false
        private bool bHasMetadata = false;
        public bool HasMetadata
        {
            get { return bHasMetadata; }
            set { bHasMetadata = value; }
        }

        public UnitUpload UnitUpload = new UnitUpload();

        public List<UploadCorrection> UploadCorrections = new List<UploadCorrection>();

        public int CorrectionCount
        {
            get { return UploadCorrections.Count; }
        }

        public bool HasCorrections
        {
            get { return (this.CorrectionCount == 0 ? false : true); }
        }

        public Unit()
        {

        }

        public string ValidateProperty(string propertyName)
        {
            return this.OnValidate(propertyName);
        }
    }

    public class UnitUpload
    {
        public UnitUpload()
        {
            estimated_flag = 0;
        }
        public int contract_id { get; set; }

        public int report_id { get; set; }

        public int mso_id { get; set; }

        public int cs_id { get; set; }

        public int unit_type_id { get; set; }

        public int unit_md_id { get; set; }

        public decimal amount { get; set; }

        public Int16 unit_period_type { get; set; }

        public string product_code { get; set; }

        public Int16 estimated_flag { get; private set; }

        public DateTime service_period_start { get; set; }

        public DateTime service_period_end { get; set; }

        public int dest_country_id { get; set; }

        public int manu_country_id { get; set; }

        public int model_id { get; set; }

        public string model { get; set; }

        public int tech_id { get; set; }

        public int manufacturer_product_id { get; set; }

        public int brand_id { get; set; }

        public string brand { get; set; }

        public int subscriber_id { get; set; }

        public int ancillary_id { get; set; }

        public int data_service_type_id { get; set; }

        public int software_id { get; set; }

        public int replicator_id { get; set; }

        public int title_id { get; set; }

        public int oem_id { get; set; }

        public int tivo_count_id { get; set; }

        public string tivo_count_description { get; set; }
    }

    public class UploadCorrection
    {
        public bool ManuallyCorrected { get; set; }
        public bool AutoCorrected { get; set; }
        public bool HasMultiplePossibleMatches { get; set; }
        public bool HasUnreconciledData { get; set; }
        public bool HasValidationError { get; set; }
        public string PropertyName { get; set; }
        public string OriginalValue { get; set; }
        public string CorrectedValue { get; set; }
        public string ManuallyCorrectedValue { get; set; }

        public UploadCorrection()
        {
            AutoCorrected = false;
            HasMultiplePossibleMatches = false;
            HasUnreconciledData = false;
            HasValidationError = false;
            ManuallyCorrected = false;
        }
    }

    public class UnitUploadReturn
    {
        public dynamic Upload { get; set; }
        public bool Inserted { get; set; }
        public string Error { get; set; }
    }

    public class UnitVerificaiton
    {
        //unit data
        List<UnitUploadCorrectionSource> UnitUploadCorrectionSource = new List<UnitUploadCorrectionSource>();
        List<ContractVerificationSource> ContractVerificationSource = new List<ContractVerificationSource>();
        List<ReportVerificationSource> ReportVerificationSource = new List<ReportVerificationSource>();
        List<MSOVerificationSource> MSOVerificationSource = new List<MSOVerificationSource>();
        List<SystemVerificationSource> SystemVerificationSource = new List<SystemVerificationSource>();
        List<UnitTypeVerificationSource> UnitTypeVerificationSource = new List<UnitTypeVerificationSource>();
        List<ProductCodeVerificationSource> ProductCodeVerificationSource = new List<ProductCodeVerificationSource>();
        //unit metadata
        List<CountryVerificationSource> CountryVerificationSource = new List<CountryVerificationSource>();
        List<ModelVerificationSource> ModelVerificationSource = new List<ModelVerificationSource>();
        List<TechnologyVerificationSource> TechnologyVerificationSource = new List<TechnologyVerificationSource>();
        List<ManufacturerProductVerificationSource> ManufacturerProductVerificationSource = new List<ManufacturerProductVerificationSource>();
        List<BrandVerificationSource> BrandVerificationSource = new List<BrandVerificationSource>();
        List<SubscriberVerificationSource> SubscriberVerificationSource = new List<SubscriberVerificationSource>();
        List<AncillaryVerificationSource> AncillaryVerificationSource = new List<AncillaryVerificationSource>();
        List<TitleVerificationSource> TitleVerificationSource = new List<TitleVerificationSource>();
        List<UnitTypeDescriptionVerificationSource> UnitTypeDescriptionVerificationSource = new List<UnitTypeDescriptionVerificationSource>();
        List<DataServiceTypeVerificationSource> DataServiceTypeVerificationSource = new List<DataServiceTypeVerificationSource>();
        List<ReplicatorVerificationSource> ReplicatorVerificationSource = new List<ReplicatorVerificationSource>();
        List<OEMVerificationSource> OEMVerificationSource = new List<OEMVerificationSource>();
        List<SoftwareVerificationSource> SoftwareVerificationSource = new List<SoftwareVerificationSource>();
        List<TivoCountVerificationSource> TivoCountVerificationSource = new List<TivoCountVerificationSource>();



        public bool IsVerificationDataLoaded { get; private set; }

        public UnitVerificaiton(DataSet ds)
        {
            //verify tables exist
            if (VerifyTablesExist(ds))
            {
                //Copy data into corresponding class objects
                //objects are easier to search than datatables
                DataTable dt = null;

                dt = ds.Tables["UnitUploadCorrection"];
                foreach (DataRow row in dt.Rows)
                {
                    UnitUploadCorrectionSource.Add(
                        new UnitUploadCorrectionSource
                        {
                            correction_id = Convert.ToInt32(row["correction_id"]),
                            field = row["field"].ToString().Trim(),
                            wrong_value = row["wrong_value"].ToString().Trim(),
                            correct_value = row["correct_value"].ToString().Trim()
                        });
                }

                dt = ds.Tables["Contract"];
                foreach (DataRow row in dt.Rows)
                {
                    ContractVerificationSource.Add(
                        new ContractVerificationSource
                        {
                            contract_id = Convert.ToInt32(row["contract_id"]),
                            bill_mso_id = Convert.ToInt32(row["bill_mso_id"]),
                            contract_description = row["contract_description"].ToString().Trim()
                        });
                }

                dt = ds.Tables["Report"];
                foreach (DataRow row in dt.Rows)
                {
                    ReportVerificationSource.Add(
                        new ReportVerificationSource
                        {
                            report_id = Convert.ToInt32(row["report_id"]),
                            contract_id = Convert.ToInt32(row["contract_id"])
                        });
                }

                dt = ds.Tables["MSO"];
                foreach (DataRow row in dt.Rows)
                {
                    MSOVerificationSource.Add(
                        new MSOVerificationSource
                        {
                            mso_id = Convert.ToInt32(row["mso_id"]),
                            name = row["name"].ToString().Trim()
                        });
                }

                dt = ds.Tables["CableSystem"];
                foreach (DataRow row in dt.Rows)
                {
                    SystemVerificationSource.Add(
                        new SystemVerificationSource
                        {
                            cs_id = Convert.ToInt32(row["cs_id"]),
                            name = row["name"].ToString().Trim(),
                            contract_id = Convert.ToInt32(row["contract_id"])
                        });
                }

                dt = ds.Tables["UnitType"];
                foreach (DataRow row in dt.Rows)
                {
                    UnitTypeVerificationSource.Add(
                        new UnitTypeVerificationSource
                        {
                            entity_level_unit_flag = Convert.ToInt32(row["entity_level_unit_flag"]),
                            unit_type_id = Convert.ToInt32(row["unit_type_id"]),
                            unit_description = row["unit_description"].ToString().Trim()
                        });
                }

                dt = ds.Tables["ProductCode"];
                foreach (DataRow row in dt.Rows)
                {
                    ProductCodeVerificationSource.Add(
                        new ProductCodeVerificationSource
                        {
                            product_code = row["product_code"].ToString().Trim(),
                            product_description = row["product_description"].ToString().Trim()
                        });
                }

                dt = ds.Tables["Country"];
                foreach (DataRow row in dt.Rows)
                {
                    CountryVerificationSource.Add(
                        new CountryVerificationSource
                        {
                            country_id = Convert.ToInt32(row["country_id"]),
                            country = row["country"].ToString().Trim()
                        });
                }

                dt = ds.Tables["Model"];
                foreach (DataRow row in dt.Rows)
                {
                    ModelVerificationSource.Add(
                        new ModelVerificationSource
                        {
                            model_id = Convert.ToInt32(row["model_id"]),
                            model = row["model_description"].ToString().Trim(),
                            //product_code = row["product_code"].ToString().Trim()
                        });
                }

                dt = ds.Tables["Technology"];
                foreach (DataRow row in dt.Rows)
                {
                    TechnologyVerificationSource.Add(
                        new TechnologyVerificationSource
                        {
                            technology_id = Convert.ToInt32(row["technology_id"]),
                            technology = row["technology"].ToString().Trim()
                        });
                }

                dt = ds.Tables["ManufacturerProduct"];
                foreach (DataRow row in dt.Rows)
                {
                    ManufacturerProductVerificationSource.Add(
                        new ManufacturerProductVerificationSource
                        {
                            manufacturer_product_id = Convert.ToInt32(row["manufacturer_product_id"]),
                            description = row["description"].ToString().Trim()
                        });
                }

                dt = ds.Tables["Brand"];
                foreach (DataRow row in dt.Rows)
                {
                    BrandVerificationSource.Add(
                        new BrandVerificationSource
                        {
                            brand_id = Convert.ToInt32(row["brand_id"]),
                            brand = row["brand"].ToString().Trim()
                        });
                }

                dt = ds.Tables["Subscriber"];
                foreach (DataRow row in dt.Rows)
                {
                    SubscriberVerificationSource.Add(
                        new SubscriberVerificationSource
                        {
                            subscriber_id = Convert.ToInt32(row["subscriber_id"]),
                            subscriber_name = row["subscriber_name"].ToString().Trim()
                        });
                }

                dt = ds.Tables["Ancillary"];
                foreach (DataRow row in dt.Rows)
                {
                    AncillaryVerificationSource.Add(
                        new AncillaryVerificationSource
                        {
                            ancillary_id = Convert.ToInt32(row["ancillary_id"]),
                            ancillary_name = row["ancillary_name"].ToString().Trim()
                        });
                }

                dt = ds.Tables["UnitTypeDescription"];
                foreach (DataRow row in dt.Rows)
                {
                    UnitTypeDescriptionVerificationSource.Add(
                        new UnitTypeDescriptionVerificationSource
                        {
                            unit_type_id = Convert.ToInt32(row["unit_type_id"]),
                            unit_description = row["unit_description"].ToString().Trim()
                        });
                }

                dt = ds.Tables["DataServiceType"];
                foreach (DataRow row in dt.Rows)
                {
                    DataServiceTypeVerificationSource.Add(
                        new DataServiceTypeVerificationSource
                        {
                            data_service_type_id = Convert.ToInt32(row["data_service_type_id"]),
                            data_service_description = row["data_service_description"].ToString().Trim()
                        });
                }

                dt = ds.Tables["Replicator"];
                foreach (DataRow row in dt.Rows)
                {
                    ReplicatorVerificationSource.Add(
                        new ReplicatorVerificationSource
                        {
                            replicator_id = Convert.ToInt32(row["replicator_id"]),
                            replicator = row["replicator"].ToString().Trim()
                        });
                }

                dt = ds.Tables["Title"];
                foreach (DataRow row in dt.Rows)
                {
                    TitleVerificationSource.Add(
                        new TitleVerificationSource
                        {
                            title_id = Convert.ToInt32(row["title_id"]),
                            title = row["title"].ToString().Trim()
                        });
                }

                dt = ds.Tables["Software"];
                foreach (DataRow row in dt.Rows)
                {
                    SoftwareVerificationSource.Add(
                        new SoftwareVerificationSource
                        {
                            software_id = Convert.ToInt32(row["software_id"]),
                            software = row["software_name"].ToString().Trim()
                        });
                }

                dt = ds.Tables["OEM"];
                foreach (DataRow row in dt.Rows)
                {
                    OEMVerificationSource.Add(
                        new OEMVerificationSource
                        {
                            oem_id = Convert.ToInt32(row["oem_id"]),
                            oem = row["oem"].ToString().Trim()
                        });
                }

                dt = ds.Tables["TivoCount"];
                foreach (DataRow row in dt.Rows)
                {
                   TivoCountVerificationSource.Add(
                        new TivoCountVerificationSource
                        {
                            tivo_count_id = Convert.ToInt32(row["tivo_count_id"]),
                            tivo_count_description = row["tivo_count_description"].ToString().Trim(),
                        });
                }

                IsVerificationDataLoaded = true;
            }
        }

        private bool VerifyTablesExist(DataSet ds)
        {
            var TF = true;
            if (TF && ds.Tables.IndexOf("UnitUploadCorrection") == -1) { TF = false; }
            if (TF && ds.Tables.IndexOf("Contract") == -1) { TF = false; }
            if (TF && ds.Tables.IndexOf("Report") == -1) { TF = false; }
            if (TF && ds.Tables.IndexOf("Mso") == -1) { TF = false; }
            if (TF && ds.Tables.IndexOf("CableSystem") == -1) { TF = false; }
            if (TF && ds.Tables.IndexOf("UnitType") == -1) { TF = false; }
            if (TF && ds.Tables.IndexOf("ProductCode") == -1) { TF = false; }
            if (TF && ds.Tables.IndexOf("Country") == -1) { TF = false; }
            if (TF && ds.Tables.IndexOf("Model") == -1) { TF = false; }
            if (TF && ds.Tables.IndexOf("Technology") == -1) { TF = false; }
            if (TF && ds.Tables.IndexOf("ManufacturerProduct") == -1) { TF = false; }
            if (TF && ds.Tables.IndexOf("Brand") == -1) { TF = false; }
            if (TF && ds.Tables.IndexOf("Subscriber") == -1) { TF = false; }
            if (TF && ds.Tables.IndexOf("Ancillary") == -1) { TF = false; }
            if (TF && ds.Tables.IndexOf("Title") == -1) { TF = false; }
            if (TF && ds.Tables.IndexOf("Replicator") == -1) { TF = false; }
            if (TF && ds.Tables.IndexOf("OEM") == -1) { TF = false; }
            if (TF && ds.Tables.IndexOf("Software") == -1) { TF = false; }
            if (TF && ds.Tables.IndexOf("UnitTypeDescription") == -1) { TF = false; }
            if (TF && ds.Tables.IndexOf("DataServiceType") == -1) { TF = false; }
            if (TF && ds.Tables.IndexOf("TivoCount") == -1) { TF = false; }
            return TF;
        }

        private T AutoCorrectValue<T>(string propertyName, T value, Unit Unit)
        {
            List<UnitUploadCorrectionSource> CorrectedValues = (from f in UnitUploadCorrectionSource
                                                                where f.field == propertyName
                                                                && f.wrong_value == value.ToString()
                                                                select f).ToList();

            foreach (UnitUploadCorrectionSource CorrectedValue in CorrectedValues)
            {
                Unit.UploadCorrections.Add(
                    new UploadCorrection
                    {
                        AutoCorrected = true,
                        CorrectedValue = CorrectedValue.ToString(),
                        OriginalValue = value.ToString(),
                        PropertyName = propertyName
                    });

                value = (T)Convert.ChangeType(CorrectedValue.correct_value, typeof(T));
            }

            return value;
        }

        private UploadCorrection NewMultipleUploadCorrection(string propertyName, string originalValue, string correctedValue)
        {
            return new UploadCorrection
                {
                    HasMultiplePossibleMatches = true,
                    OriginalValue = originalValue,
                    CorrectedValue = correctedValue,
                    PropertyName = propertyName
                };
        }

        public void Verify(Unit Unit, List<string> PropertyList, bool ClearCorrections = true, UploadCorrection upc = null)
        {
            if (ClearCorrections)
            {
                Unit.UploadCorrections = new List<UploadCorrection>();
            }

            foreach (var propertyName in PropertyList)
            {
                switch (propertyName)
                {
                    case "contract_description":
                    case "cs_name":
                    case "mso_name":
                    case "report_description":
                    case "estimated_flag":
                        //do nothing description only
                        break;
                    case "amount":
                        Unit.UnitUpload.amount = AutoCorrectValue<Decimal>(propertyName, Unit.amount, Unit);
                        break;

                    case "ancillary":
                        Unit.ancillary = AutoCorrectValue<string>(propertyName, Unit.ancillary, Unit);
                        List<AncillaryVerificationSource> ancillary = (from x in AncillaryVerificationSource
                                                                       where string.Equals(x.ancillary_name, Unit.ancillary, StringComparison.OrdinalIgnoreCase)
                                                                       select x).ToList();
                        
                        if (ancillary.Count == 1)
                        {
                            Unit.UnitUpload.ancillary_id = ancillary[0].ancillary_id;
                            //DWR-Added 5/7/12 - Signifies that the unit has at least one metadata value
                            Unit.HasMetadata = true;
                        }
                        else if (ancillary.Count == 0 && !string.IsNullOrEmpty(Unit.ancillary))
                        {
                            Unit.UploadCorrections.Add(
                            new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.ancillary });
                        }
                        else if (!string.IsNullOrEmpty(Unit.ancillary))
                        {
                            if (upc == null)
                            {
                                foreach (AncillaryVerificationSource ancillaryItem in ancillary)
                                {
                                    Unit.UploadCorrections.Add(
                                        NewMultipleUploadCorrection(propertyName, Unit.ancillary, ancillaryItem.ancillary_id.ToString()));
                                }
                            }
                            else
                            {
                                List<AncillaryVerificationSource> muAncillary = (from x in ancillary
                                                                                 where x.ancillary_id.ToString() == upc.ManuallyCorrectedValue
                                                                                 select x).ToList();
                                if (muAncillary.Count == 1)
                                {
                                    Unit.UnitUpload.ancillary_id = muAncillary[0].ancillary_id;
                                    Unit.UploadCorrections.Remove(upc);
                                }
                            }
                        }
                        break;

                    case "brand":
                        Unit.brand = AutoCorrectValue<string>(propertyName, Unit.brand, Unit);
                        List<BrandVerificationSource> brand = (from x in BrandVerificationSource
                                                               where string.Equals(x.brand, Unit.brand, StringComparison.OrdinalIgnoreCase)
                                                               select x).ToList();

                        if (brand.Count == 1)
                        {
                            Unit.UnitUpload.brand_id = brand[0].brand_id;
                            //DWR-Added 5/7/12 - Signifies that the unit has at least one metadata value
                            Unit.HasMetadata = true;
                        }
                        else if (brand.Count == 0 && !string.IsNullOrEmpty(Unit.brand))
                        {
                            Unit.UploadCorrections.Add(
                                new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.brand });
                            Unit.UnitUpload.brand = Unit.brand;
                        }
                        else if (!string.IsNullOrEmpty(Unit.brand))
                        {
                            if (upc == null)
                            {
                                foreach (BrandVerificationSource brandItem in brand)
                                {
                                    Unit.UploadCorrections.Add(
                                        NewMultipleUploadCorrection(propertyName, Unit.brand, brandItem.brand_id.ToString()));
                                }
                            }
                            else
                            {
                                List<BrandVerificationSource> muBrand = (from x in brand
                                                                         where x.brand_id.ToString() == upc.ManuallyCorrectedValue
                                                                         select x).ToList();

                                if (muBrand.Count == 1)
                                {
                                    Unit.UnitUpload.brand_id = muBrand[0].brand_id;
                                    Unit.UploadCorrections.Remove(upc);
                                }
                            }
                        }
                        break;

                    case "contract_id":
                        List<ContractVerificationSource> Contracts = (from x in ContractVerificationSource where x.contract_id == Unit.contract_id select x).ToList();

                        if (Contracts.Count == 1)
                        {
                            Unit.UnitUpload.contract_id = Contracts[0].contract_id;
                            Unit.UnitUpload.mso_id = Contracts[0].bill_mso_id;
                        }
                        else if (Contracts.Count == 0)
                        {
                            Unit.UploadCorrections.Add(
                                new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.contract_id.ToString() });
                        }
                        break;

                    case "cs_id":
                        if (Unit.cs_id == 0)
                        {
                                Unit.UnitUpload.cs_id = Unit.cs_id;
                        }
                        else
                        {
                            List<SystemVerificationSource> cableSystems = (from x in SystemVerificationSource
                                                                           where x.cs_id == Unit.cs_id
                                                                           && x.contract_id == Unit.UnitUpload.contract_id
                                                                           select x).ToList();

                            if (cableSystems.Count == 1)
                            {
                                Unit.UnitUpload.cs_id = cableSystems[0].cs_id;
                            }
                            else if (cableSystems.Count == 0)
                            {
                                Unit.UploadCorrections.Add(
                                    new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.cs_id.ToString() });
                            }


                        }
                        break;

                    case "data":
                        Unit.data = AutoCorrectValue<string>(propertyName, Unit.data, Unit);
                        List<DataServiceTypeVerificationSource> dataServiceTypes = (from x in DataServiceTypeVerificationSource
                                                                                    where string.Equals(x.data_service_description, Unit.data, StringComparison.OrdinalIgnoreCase)
                                                                                    select x).ToList();
                        if (dataServiceTypes.Count == 1)
                        {
                            Unit.UnitUpload.data_service_type_id = dataServiceTypes[0].data_service_type_id;
                            //DWR-Added 5/7/12 - Signifies that the unit has at least one metadata value
                            Unit.HasMetadata = true;
                        }
                        else if (dataServiceTypes.Count == 0 && !string.IsNullOrEmpty(Unit.data))
                        {
                            Unit.UploadCorrections.Add(
                                new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.data });
                        }
                        else if (!string.IsNullOrEmpty(Unit.data))
                        {
                            foreach (DataServiceTypeVerificationSource dataServiceType in dataServiceTypes)
                            {
                                Unit.UploadCorrections.Add(
                                    NewMultipleUploadCorrection(propertyName, Unit.data, dataServiceType.data_service_type_id.ToString()));
                            }
                        }
                        break;

                    case "destination country":
                        Unit.destination_country = AutoCorrectValue<string>(propertyName, Unit.destination_country, Unit);
                        List<CountryVerificationSource> countries_dest = (from x in CountryVerificationSource
                                                                          where string.Equals(x.country, Unit.destination_country, StringComparison.OrdinalIgnoreCase)
                                                                          select x).ToList();
                        if (countries_dest.Count == 1)
                        {
                            Unit.UnitUpload.dest_country_id = countries_dest[0].country_id;
                            //DWR-Added 5/7/12 - Signifies that the unit has at least one metadata value
                            Unit.HasMetadata = true;
                        }
                        else if (countries_dest.Count == 0 && !string.IsNullOrEmpty(Unit.destination_country))
                        {
                            Unit.UploadCorrections.Add(
                                new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.destination_country });
                        }
                        else if (!string.IsNullOrEmpty(Unit.destination_country))
                        {
                            foreach (CountryVerificationSource countryItem in countries_dest)
                            {
                                Unit.UploadCorrections.Add(
                                    NewMultipleUploadCorrection(propertyName, Unit.destination_country, countryItem.country_id.ToString()));
                            }
                        }
                        break;

                    case "manufactured country":
                        Unit.manufactured_country = AutoCorrectValue<string>(propertyName, Unit.manufactured_country, Unit);
                        List<CountryVerificationSource> countries_manu = (from x in CountryVerificationSource
                                                                          where string.Equals(x.country, Unit.manufactured_country, StringComparison.OrdinalIgnoreCase)
                                                                          select x).ToList();
                        if (countries_manu.Count == 1)
                        {
                            Unit.UnitUpload.manu_country_id = countries_manu[0].country_id;
                            //DWR-Added 5/7/12 - Signifies that the unit has at least one metadata value
                            Unit.HasMetadata = true;
                        }
                        else if (countries_manu.Count == 0 && !string.IsNullOrEmpty(Unit.manufactured_country))
                        {
                            Unit.UploadCorrections.Add(
                                new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.manufactured_country });
                        }
                        else if (!string.IsNullOrEmpty(Unit.manufactured_country))
                        {
                            foreach (CountryVerificationSource countryItem in countries_manu)
                            {
                                Unit.UploadCorrections.Add(
                                    NewMultipleUploadCorrection(propertyName, Unit.manufactured_country, countryItem.country_id.ToString()));
                            }
                        }
                        break;

                    case "manufactured product":
                        Unit.manufactured_product = AutoCorrectValue<string>(propertyName, Unit.manufactured_product, Unit);
                        List<ManufacturerProductVerificationSource> manufacturer_products = (from x in ManufacturerProductVerificationSource
                                                                                             where string.Equals(x.description, Unit.manufactured_product, StringComparison.OrdinalIgnoreCase)
                                                                                             select x).ToList();
                        if (manufacturer_products.Count == 1)
                        {
                            Unit.UnitUpload.manufacturer_product_id = manufacturer_products[0].manufacturer_product_id;
                            //DWR-Added 5/7/12 - Signifies that the unit has at least one metadata value
                            Unit.HasMetadata = true;
                        }
                        else if (manufacturer_products.Count == 0 && !string.IsNullOrEmpty(Unit.manufactured_product))
                        {
                            Unit.UploadCorrections.Add(
                                new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.manufactured_product });
                        }
                        else if (!string.IsNullOrEmpty(Unit.manufactured_product))
                        {
                            foreach (ManufacturerProductVerificationSource product in manufacturer_products)
                            {
                                Unit.UploadCorrections.Add(
                                    NewMultipleUploadCorrection(propertyName, Unit.manufactured_product, product.manufacturer_product_id.ToString()));
                            }
                        }
                        break;

                    case "model":
                        Unit.model = AutoCorrectValue<string>(propertyName, Unit.model, Unit);
                       
                           List<ModelVerificationSource> models = (from x in ModelVerificationSource
                                      where string.Equals(x.model, Unit.model, StringComparison.OrdinalIgnoreCase)
                                      //&& string.Equals(x.product_code.Trim(), Unit.product_code.Trim(), StringComparison.OrdinalIgnoreCase)
                                      select x).ToList();
           

                        if (models.Count == 1)
                        {
                            Unit.UnitUpload.model_id = models[0].model_id;
                            Unit.UnitUpload.model = models[0].model;
                            //DWR-Added 5/7/12 - Signifies that the unit has at least one metadata value
                            Unit.HasMetadata = true;
                        }
                        else if (models.Count == 0 && !string.IsNullOrEmpty(Unit.model))
                        {
                            Unit.UploadCorrections.Add(
                                new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.model });
                            Unit.UnitUpload.model = Unit.model;
                        }
                        else if (!string.IsNullOrEmpty(Unit.model))
                        {
                            foreach (ModelVerificationSource model in models)
                            {
                                Unit.UploadCorrections.Add(
                                    NewMultipleUploadCorrection(propertyName, Unit.model, model.model_id.ToString()));
                            }
                        }
                        break;

                      case "oem":
                        Unit.oem = AutoCorrectValue<string>(propertyName, Unit.oem, Unit);
                        List<OEMVerificationSource> oems = (from x in OEMVerificationSource
                                                                where string.Equals(x.oem, Unit.oem, StringComparison.OrdinalIgnoreCase)
                                                                //&& string.Equals(x.product_code.Trim(), Unit.product_code.Trim(), StringComparison.OrdinalIgnoreCase)
                                                                select x).ToList();

                        if (oems.Count == 1)
                        {
                            Unit.UnitUpload.oem_id = oems[0].oem_id;
                            //DWR-Added 5/7/12 - Signifies that the unit has at least one metadata value
                            Unit.HasMetadata = true;
                        }
                        else if (oems.Count == 0 && !string.IsNullOrEmpty(Unit.oem))
                        {
                            Unit.UploadCorrections.Add(
                                new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.oem });
                        }
                        else if (!string.IsNullOrEmpty(Unit.oem))
                        {
                            foreach (OEMVerificationSource oem in oems)
                            {
                                Unit.UploadCorrections.Add(
                                    NewMultipleUploadCorrection(propertyName, Unit.oem, oem.oem_id.ToString()));
                            }
                        }
                        break;

                    case "replicator":
                        Unit.replicator = AutoCorrectValue<string>(propertyName, Unit.replicator, Unit);
                        List<ReplicatorVerificationSource> replicators = (from x in ReplicatorVerificationSource
                                                                where string.Equals(x.replicator, Unit.replicator, StringComparison.OrdinalIgnoreCase)
                                                                //&& string.Equals(x.product_code.Trim(), Unit.product_code.Trim(), StringComparison.OrdinalIgnoreCase)
                                                                select x).ToList();

                        if (replicators.Count == 1)
                        {
                            Unit.UnitUpload.replicator_id = replicators[0].replicator_id;
                            //DWR-Added 5/7/12 - Signifies that the unit has at least one metadata value
                            Unit.HasMetadata = true;
                        }
                        else if (replicators.Count == 0 && !string.IsNullOrEmpty(Unit.replicator))
                        {
                            Unit.UploadCorrections.Add(
                                new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.replicator });
                        }
                        else if (!string.IsNullOrEmpty(Unit.replicator))
                        {
                            foreach (ReplicatorVerificationSource replicator in replicators)
                            {
                                Unit.UploadCorrections.Add(
                                    NewMultipleUploadCorrection(propertyName, Unit.replicator, replicator.replicator_id.ToString()));
                            }
                        }
                        break;
                    case "title":
                        Unit.title = AutoCorrectValue<string>(propertyName, Unit.title, Unit);
                        List<TitleVerificationSource> titles = (from x in TitleVerificationSource
                                                                where string.Equals(x.title, Unit.title, StringComparison.OrdinalIgnoreCase)
                                                                //&& string.Equals(x.product_code.Trim(), Unit.product_code.Trim(), StringComparison.OrdinalIgnoreCase)
                                                                select x).ToList();

                        if (titles.Count == 1)
                        {
                            Unit.UnitUpload.title_id = titles[0].title_id;
                            //DWR-Added 5/7/12 - Signifies that the unit has at least one metadata value
                            Unit.HasMetadata = true;
                        }
                        else if (titles.Count == 0 && !string.IsNullOrEmpty(Unit.title))
                        {
                            Unit.UploadCorrections.Add(
                                new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.title });
                        }
                        else if (!string.IsNullOrEmpty(Unit.title))
                        {
                            foreach (TitleVerificationSource title in titles)
                            {
                                Unit.UploadCorrections.Add(
                                    NewMultipleUploadCorrection(propertyName, Unit.title, title.title_id.ToString()));
                            }
                        }
                        break;

                    case "software":
                        Unit.software = AutoCorrectValue<string>(propertyName, Unit.software, Unit);
                        List<SoftwareVerificationSource> softwares = (from x in SoftwareVerificationSource
                                                                where string.Equals(x.software, Unit.software, StringComparison.OrdinalIgnoreCase)
                                                                //&& string.Equals(x.product_code.Trim(), Unit.product_code.Trim(), StringComparison.OrdinalIgnoreCase)
                                                                select x).ToList();

                        if (softwares.Count == 1)
                        {
                            Unit.UnitUpload.software_id = softwares[0].software_id;
                            //DWR-Added 5/7/12 - Signifies that the unit has at least one metadata value
                            Unit.HasMetadata = true;
                        }
                        else if (softwares.Count == 0 && !string.IsNullOrEmpty(Unit.software))
                        {
                            Unit.UploadCorrections.Add(
                                new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.software });
                        }
                        else if (!string.IsNullOrEmpty(Unit.software))
                        {
                            foreach (SoftwareVerificationSource software in softwares)
                            {
                                Unit.UploadCorrections.Add(
                                    NewMultipleUploadCorrection(propertyName, Unit.software, software.software_id.ToString()));
                            }
                        }
                        break;

                    case "tivo count description":
                        Unit.tivo_count_description = AutoCorrectValue<string>(propertyName, Unit.tivo_count_description, Unit);

                        List<TivoCountVerificationSource> tivocounts = (from x in TivoCountVerificationSource
                                                                where string.Equals(x.tivo_count_description, Unit.tivo_count_description, StringComparison.OrdinalIgnoreCase)
                                                                select x).ToList();


                        if (tivocounts.Count == 1)
                        {
                            Unit.UnitUpload.tivo_count_id = tivocounts[0].tivo_count_id;
                            Unit.UnitUpload.tivo_count_description = tivocounts[0].tivo_count_description;
                            //DWR-Added 5/7/12 - Signifies that the unit has at least one metadata value
                            Unit.HasMetadata = true;
                        }
                        else if (tivocounts.Count == 0 && !string.IsNullOrEmpty(Unit.tivo_count_description))
                        {
                            Unit.UploadCorrections.Add(
                                new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.tivo_count_description });
                            Unit.UnitUpload.tivo_count_description = Unit.tivo_count_description;
                        }
                        else if (!string.IsNullOrEmpty(Unit.tivo_count_description))
                        {
                            foreach (TivoCountVerificationSource tivo_count_description in tivocounts)
                            {
                                Unit.UploadCorrections.Add(
                                    NewMultipleUploadCorrection(propertyName, Unit.tivo_count_description, tivo_count_description.tivo_count_id.ToString()));
                            }
                        }
                        break;
                    //case "mso_id":
                    //    Unit.mso_name = AutoCorrectValue<string>(propertyName, Unit.mso_name, Unit);
                    //    List<MSOVerificationSource> msoes = (from x in MSOVerificationSource where x.mso_id == Unit.mso_id select x).ToList();
                    //    if (msoes.Count == 1)
                    //    {
                    //        Unit.UnitUpload.mso_id = msoes[0].mso_id;
                    //    }
                    //    else if (msoes.Count == 0)
                    //    {
                    //        Unit.UploadCorrections.Add(
                    //            new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.mso_id.ToString() });
                    //    }
                    //    break;

                    case "product_code":
                        Unit.product_code = AutoCorrectValue<string>(propertyName, Unit.product_code, Unit);
                        List<ProductCodeVerificationSource> products = new List<ProductCodeVerificationSource>();
                        if (Unit.product_code != null)
                        {
                            products = (from x in ProductCodeVerificationSource
                                        where string.Equals(x.product_code.Trim(), Unit.product_code.Trim(), StringComparison.OrdinalIgnoreCase)
                                        select x).ToList();
                        }

                        if (products.Count == 1)
                        {
                            Unit.UnitUpload.product_code = products[0].product_code;
                        }
                        else if (products.Count == 0 && !string.IsNullOrEmpty(Unit.product_code))
                        {
                            Unit.UploadCorrections.Add(
                                new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.product_code });
                        }
                        else if (!string.IsNullOrEmpty(Unit.product_code))
                        {
                            foreach (ProductCodeVerificationSource product in products)
                            {
                                Unit.UploadCorrections.Add(
                                    NewMultipleUploadCorrection(propertyName, Unit.product_code, product.product_code.ToString()));
                            }
                        }
                        break;

                    case "report_id":
                        if (Unit.report_id == 0)
                        {
                            Unit.UnitUpload.report_id = Unit.report_id;
                        }
                        else
                        {
                            List<ReportVerificationSource> reports = (from x in ReportVerificationSource
                                                                      where x.report_id == Unit.report_id
                                                                      && x.contract_id == Unit.UnitUpload.contract_id
                                                                      select x).ToList();
                            if (reports.Count == 1)
                            {
                                Unit.UnitUpload.report_id = reports[0].report_id;
                            }
                            else if (reports.Count == 0)
                            {
                                Unit.UploadCorrections.Add(
                                    new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.report_id.ToString() });
                            }
                        }
                        break;

                    case "service_period_end":
                        Unit.UnitUpload.service_period_end = Unit.service_period_end;
                        break;

                    case "service_period_start":
                        Unit.UnitUpload.service_period_start = Unit.service_period_start;
                        break;

                    case "subscriber":
                        Unit.subscriber = AutoCorrectValue<string>(propertyName, Unit.subscriber, Unit);
                        List<SubscriberVerificationSource> subscribers = (from x in SubscriberVerificationSource
                                                                          where string.Equals(x.subscriber_name, Unit.subscriber, StringComparison.OrdinalIgnoreCase)
                                                                          select x).ToList();
                        if (subscribers.Count == 1)
                        {
                            Unit.UnitUpload.subscriber_id = subscribers[0].subscriber_id;
                            //DWR-Added 5/7/12 - Signifies that the unit has at least one metadata value
                            Unit.HasMetadata = true;
                        }
                        else if (subscribers.Count == 0 && !string.IsNullOrEmpty(Unit.subscriber))
                        {
                            Unit.UploadCorrections.Add(
                                new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.subscriber });
                        }
                        else if (!string.IsNullOrEmpty(Unit.subscriber))
                        {
                            foreach (SubscriberVerificationSource subscriber in subscribers)
                            {
                                Unit.UploadCorrections.Add(
                                    NewMultipleUploadCorrection(propertyName, Unit.subscriber, subscriber.subscriber_id.ToString()));
                            }
                        }
                        break;

                    case "technology":
                        Unit.technology = AutoCorrectValue<string>(propertyName, Unit.technology, Unit);
                        List<TechnologyVerificationSource> technologies = (from x in TechnologyVerificationSource
                                                                           where string.Equals(x.technology, Unit.technology, StringComparison.OrdinalIgnoreCase)
                                                                           select x).ToList();
                        if (technologies.Count == 1)
                        {
                            Unit.UnitUpload.tech_id = technologies[0].technology_id;
                            //DWR-Added 5/7/12 - Signifies that the unit has at least one metadata value
                            Unit.HasMetadata = true;
                        }
                        else if (technologies.Count == 0 && !string.IsNullOrEmpty(Unit.technology))
                        {
                            Unit.UploadCorrections.Add(
                                new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.technology });
                        }
                        else if (!string.IsNullOrEmpty(Unit.technology))
                        {
                            foreach (TechnologyVerificationSource technology_id in technologies)
                            {
                                Unit.UploadCorrections.Add(
                                    NewMultipleUploadCorrection(propertyName, Unit.technology, technology_id.technology_id.ToString()));
                            }
                        }
                        break;

                    case "unit_period_type_description":
                        Unit.unit_period_type_description = AutoCorrectValue<string>(propertyName, Unit.unit_period_type_description, Unit);
                        switch (Unit.unit_period_type_description.ToLower())
                        {
                            case "actual":
                                Unit.UnitUpload.unit_period_type = 0;
                                break;
                            case "forecast":
                                Unit.UnitUpload.unit_period_type = 1;
                                break;
                            default:
                                Unit.UploadCorrections.Add(
                                    new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.unit_period_type_description });
                                break;
                        }
                        break;

                    case "unit_description":
                        Unit.unit_description = AutoCorrectValue<string>(propertyName, Unit.unit_description, Unit);
                        List<UnitTypeDescriptionVerificationSource> UnitDescriptions = (from x in UnitTypeDescriptionVerificationSource
                                                                                        where string.Equals(x.unit_description, Unit.unit_description, StringComparison.OrdinalIgnoreCase)
                                                                                        select x).ToList();
                        if (UnitDescriptions.Count == 1)
                        {
                            Unit.UnitUpload.unit_type_id = UnitDescriptions[0].unit_type_id;
                        }
                        else if (UnitDescriptions.Count == 0 && !string.IsNullOrEmpty(Unit.unit_description))
                        {
                            Unit.UploadCorrections.Add(
                                new UploadCorrection { PropertyName = propertyName, HasUnreconciledData = true, OriginalValue = Unit.unit_description });
                        }
                        else if (!string.IsNullOrEmpty(Unit.unit_description))
                        {
                            foreach (UnitTypeDescriptionVerificationSource UnitDescription in UnitDescriptions)
                            {
                                Unit.UploadCorrections.Add(
                                    NewMultipleUploadCorrection(propertyName, Unit.unit_description, UnitDescription.unit_type_id.ToString()));
                            }
                        }
                        break;



                    default:
                        break;
                }
            }
        }
    }

    #region Verification Sources

    public class UnitUploadCorrectionSource
    {
        public int correction_id { get; set; }
        public string field { get; set; }
        public string wrong_value { get; set; }
        public string correct_value { get; set; }
    }

    public class ContractVerificationSource
    {
        public int contract_id { get; set; }
        public int bill_mso_id { get; set; }
        public string contract_description { get; set; }
    }

    public class ReportVerificationSource
    {
        public int report_id { get; set; }
        public int contract_id { get; set; }
        public string description { get; set; }
    }

    public class MSOVerificationSource
    {
        public int mso_id { get; set; }
        public string name { get; set; }
    }

    public class SystemVerificationSource
    {
        public int cs_id { get; set; }
        public string name { get; set; }
        public int contract_id { get; set; }
    }

    public class UnitTypeVerificationSource
    {
        public int unit_type_id { get; set; }
        public int entity_level_unit_flag { get; set; }
        public string unit_description { get; set; }
    }

    public class ProductCodeVerificationSource
    {
        public string product_code { get; set; }
        public string product_description { get; set; }
    }

    public class CountryVerificationSource
    {
        public int country_id { get; set; }
        public string country { get; set; }
    }

    public class ModelVerificationSource
    {
        public int model_id { get; set; }
        public string model { get; set; }
        //public string product_code { get; set; }
    }

    public class TechnologyVerificationSource
    {
        public int technology_id { get; set; }
        public string technology { get; set; }
    }

    public class ManufacturerProductVerificationSource
    {
        public int manufacturer_product_id { get; set; }
        public string description { get; set; }
    }

    public class BrandVerificationSource
    {
        public int brand_id { get; set; }
        public string brand { get; set; }
    }

    public class SubscriberVerificationSource
    {
        public int subscriber_id { get; set; }
        public string subscriber_name { get; set; }
    }

    public class AncillaryVerificationSource
    {
        public int ancillary_id { get; set; }
        public string ancillary_name { get; set; }
    }

    public class TitleVerificationSource
    {
        public int title_id { get; set; }
        public string title { get; set; }
    }

    public class UnitTypeDescriptionVerificationSource
    {
        public int unit_type_id { get; set; }
        public string unit_description { get; set; }
    }

    public class DataServiceTypeVerificationSource
    {
        public int data_service_type_id { get; set; }
        public string data_service_description { get; set; }
    }

    public class ReplicatorVerificationSource
    {
        public int replicator_id { get; set; }
        public string replicator { get; set; }
    }

    public class OEMVerificationSource
    {
        public int oem_id { get; set; }
        public string oem { get; set; }
    }

    public class SoftwareVerificationSource
    {
        public int software_id { get; set; }
        public string software { get; set; }
    }

    public class TivoCountVerificationSource
    {
        public int tivo_count_id { get; set; }
        public string tivo_count_description { get; set; }
    }
    #endregion
}
