using FCASociety = Beis.HelpToGrow.Voucher.Web.Models.Voucher.FCASociety;
using FCASocietyEntity = Beis.HelpToGrow.Persistence.Models.fcasociety;


namespace Beis.HelpToGrow.Voucher.Web.Services.FCAServices
{
    public class FCASocietyService : IFCASocietyService
    {
        private readonly IFCASocietyRepository _fcaSocietyRepository;

        public FCASocietyService(IFCASocietyRepository fcaSocietyRepository)
        {
            _fcaSocietyRepository = fcaSocietyRepository;
        }

        public async Task LoadFCASocieties()
        {
            var records = LoadFromFile();

            await UpdateEntities(records);
        }
        
        public async Task<FCASociety> GetSociety(string societyNumber)
        {
            var item = await _fcaSocietyRepository.GetFCASocietyByNumber(societyNumber.Trim());

            if (item == null)
            {
                return null;
            }

            return new FCASociety
            {
                SocietyNumber = item.society_number,
                SocietySuffix = item.society_suffix,
                FullRegistrationNumber = item.full_registration_number,
                SocietyName = item.society_name,
                RegisteredAs = item.registered_as,
                SocietyAddress = item.society_address,
                RegistrationDate = item.registration_date,
                DeregistrationDate = item.deregistration_date,
                RegistrationAct = item.registration_act,
                SocietyStatus = item.society_status
            };
        }

        private static IEnumerable<FCASociety> LoadFromFile()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var embeddedResources = assembly.GetManifestResourceNames();
            var fileName = embeddedResources.First(x => x.EndsWith("SocietyList.csv"));

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.Replace(" ", "")
            };

            var manifestResourceStream = assembly.GetManifestResourceStream(fileName)
                ?? throw new NullReferenceException($"Failed reading manifest resource stream for: {fileName}");

            using var reader = new StreamReader(manifestResourceStream);
            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<FCASociety>().ToList();

            return records;
        }

        private async Task UpdateEntities(IEnumerable<FCASociety> records)
        {
            var index = 1;
            var fcaSocietyEntities = new List<FCASocietyEntity>();
            foreach (var fcaSocietyEntity in records.Select(_ => new FCASocietyEntity
            {
                societyId = index,
                society_number = _.SocietyNumber,
                society_suffix = _.SocietySuffix,
                full_registration_number = _.FullRegistrationNumber,
                society_name = _.SocietyName,
                registered_as = _.RegisteredAs,
                society_address = _.SocietyAddress,
                registration_date = _.RegistrationDate,
                deregistration_date = _.DeregistrationDate,
                registration_act = _.RegistrationAct,
                society_status = _.SocietyStatus
            }))
            {
                fcaSocietyEntities.Add(fcaSocietyEntity);

                index += 1;
            }

            await _fcaSocietyRepository.AddSocieties(fcaSocietyEntities);
        }
    }
}