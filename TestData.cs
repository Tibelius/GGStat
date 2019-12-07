using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Data;
using static GGStat.Game;
using System.Data.SQLite;

namespace GGStat {
    class TestData : Data {
        new public static string db_saveMatch = "INSERT INTO match(winner, player1_id, player2_id, player1_character_id, player2_character_id, timestamp) VALUES({0}, '{1}', {2}, {3}, {4}, {5});";
        new public static string db_saveRound = "INSERT INTO round(match_id, number, winner, time_left, player1_hp, player2_hp, timestamp) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6});";
        new public static string db_saveAlias = "INSERT INTO alias(name, player_id, timestamp) VALUES('{0}', {1}, {2});";

        private static string[] testNames = { "cgrinyakin0", "njanisson1", "hbroadbury2", "bmorena3", "mknappett4", "rtomasello5", "ctrebbett6", "pmudge7", "mdelisle8", "ltindall9", "fmcclunaghana", "tbrothwellb", "nsandalc", "gchetwindd", "blinkletere", "lpadkinf", "brosellg", "gteeseh", "lzanettinii", "epardalj", "mmarksonk", "bburgisl", "mmoraledam", "jvanyutinn", "edesborougho", "lgladstonep", "bsamuelq", "sbengtsenr", "bfernes", "slonghurstt", "sarmatageu", "bivachyovv", "rcheccucciw", "boxenfordx", "cvardony", "kkubesz", "lspeedin10", "vpeare11", "gwetherald12", "bwalicki13", "everrechia14", "bcrabbe15", "pmitrovic16", "measterfield17", "mrooze18", "earndtsen19", "dteasdalemarkie1a", "smackley1b", "gbirds1c", "xschachter1d", "xcalbert1e", "ahayden1f", "kbollon1g", "pphidgin1h", "kpuckham1i", "ecrampin1j", "eharrismith1k", "gfibben1l", "flightowler1m", "lmuzzollo1n", "ldubery1o", "sbaffin1p", "sdanelut1q", "dczaple1r", "gtomson1s", "vmarunchak1t", "rbrislane1u", "nfrostdicke1v", "mdinsale1w", "tblaksley1x", "ccansdell1y", "ebolzen1z", "sbembrigg20", "bschelle21", "mmyatt22", "lharbottle23", "vrobertot24", "aconnechie25", "ftomik26", "wbeswetherick27", "mkepp28", "amaffiotti29", "rmackissack2a", "aduffill2b", "celfitt2c", "bzanicchelli2d", "tharnetty2e", "dvalder2f", "vgalea2g", "mdescroix2h", "ftwydell2i", "htorresi2j", "srenahan2k", "rsolesbury2l", "lnottingam2m", "wsich2n", "pstoodley2o", "hfigura2p", "odeboy2q", "tseneschal2r", "slaidel2s", "aickovicz2t", "jdrewett2u", "slabbe2v", "rclougher2w", "pbarkess2x", "sbaszniak2y", "dscotting2z", "ksuttle30", "rbramer31", "tvarley32", "sbushnell33", "tballister34", "hrosengarten35", "aibell36", "hpashby37", "bmarston38", "apitrasso39", "vgriffithe3a", "plibero3b", "mfaichney3c", "opentelo3d", "lparish3e", "scarlens3f", "kgillings3g", "jtoseland3h", "tsheering3i", "astickler3j", "dantonioni3k", "kpugh3l", "ctowle3m", "pmcluckie3n", "mgamell3o", "chardingham3p", "krobroe3q", "kphippard3r", "dalbrook3s", "emoules3t", "npaddingdon3u", "smcelvogue3v", "torcott3w", "cstrettle3x", "alodwick3y", "rburch3z", "dkondratenya40", "arenshall41", "bsnowball42", "rgrandham43", "mnowlan44", "fcockett45", "rpostles46", "bpeplaw47", "gfibbens48", "agantz49", "clockwood4a", "iattack4b", "bdellcasa4c", "mleatham4d", "nfeares4e", "gburbridge4f", "pgoosey4g", "kburton4h", "cpynn4i", "alabbati4j", "pbaradel4k", "nwhickman4l", "ncrutcher4m", "lrash4n", "sgutans4o", "gwinmill4p", "cbennen4q", "pjurkowski4r", "jmcgovern4s", "sbestiman4t", "jjeayes4u", "elatan4v", "vmarcus4w", "lswiggs4x", "dasgodby4y", "kmacphee4z", "hgreensitt50", "lbrobeck51", "cwithull52", "zmclae53", "sfedynski54", "iwudeland55", "egeerling56", "lterbrug57", "dlayson58", "mbarringer59", "cbateup5a", "lprevett5b", "dneame5c", "lcomer5d", "kcurson5e", "lgierok5f", "chavelin5g", "sarundale5h", "rwellstood5i", "bguiel5j", "lschober5k", "cscandrett5l", "ddagostino5m", "lshearmer5n", "bcrossland5o", "wcroal5p", "nmattschas5q", "afolca5r", "mkynd5s", "kmathie5t", "acobden5u", "elinnell5v", "rbridgewater5w", "ksydney5x", "hlabbet5y", "hbloxland5z", "dshills60", "scroxton61", "jrockliffe62", "elarter63", "alacky64", "rsparwell65", "laldridge66", "gstadding67", "sgreenwood68", "zbreslauer69", "oworrill6a", "chiggen6b", "mbeevens6c", "maleksandrev6d", "gbrannigan6e", "gduer6f", "dsimpole6g", "cboch6h", "pdingate6i", "nbenoey6j", "lleak6k", "slethibridge6l", "blukas6m", "gcufley6n", "kmoggach6o", "scorbet6p", "cvanes6q", "flearmond6r", "tedgworth6s", "kflowerdew6t", "mteml6u", "kleverentz6v", "apinkerton6w", "knettleship6x", "jitzhaiek6y", "alloyds6z", "bbachshell70", "pbleier71", "jhoulden72", "kfryer73", "mbolton74", "beouzan75", "obrummell76", "lgasgarth77", "gazemar78", "mmorat79", "kbourne7a", "dselkirk7b", "cyashaev7c", "eblinerman7d", "bbandey7e", "fabad7f", "zlindholm7g", "smatcham7h", "bpeachment7i", "bseak7j", "cmcairt7k", "rplumptre7l", "afardoe7m", "hbolin7n", "pcanby7o", "mmayes7p", "dgillcrist7q", "mcrafter7r", "fpurry7s", "gmealham7t", "dbromell7u", "dmcnae7v", "tarnke7w", "smetts7x", "mholyard7y", "fzealey7z", "crobroe80", "msoames81", "hdewi82", "dtrencher83", "pcushworth84", "barlidge85", "tvanichkin86", "ilittler87", "awoollcott88", "hselbie89", "cbradman8a", "hbeech8b", "fgilbertson8c", "ljosefovic8d", "hmantrip8e", "jreavey8f", "mburdett8g", "rbrowell8h", "cdenrico8i", "icoull8j", "uoliff8k", "mhawke8l", "rhuelin8m", "ehuddart8n", "aharrald8o", "chandaside8p", "fcantillion8q", "gmuschette8r", "sperllman8s", "lcornbell8t", "astroud8u", "jhuton8v", "pdeverose8w", "alovart8x", "dwitherup8y", "cpryde8z", "dgatch90", "sdullingham91", "tbodesson92", "kpedrozzi93", "mabbatucci94", "rransom95", "npriestner96", "gscotcher97", "epardue98", "jgrasha99", "lpheby9a", "lbegwell9b", "lsiman9c", "spendered9d", "bbloomer9e", "cbenthall9f", "vsimnor9g", "gdyment9h", "kboddice9i", "lbeach9j", "hewing9k", "vcrewe9l", "gstocker9m", "cpaolillo9n", "lfall9o", "cclearie9p", "tslite9q", "ohalsted9r", "skelemen9s", "pgrzegorzewski9t", "cjouning9u", "ihallbird9v", "gsewall9w", "mshalcros9x", "abartzen9y", "stallow9z", "rciscoa0", "arzehora1", "mvasilika2", "bantonovica3", "bofahertya4", "cgregora5", "acranea6", "jdenecampa7", "rswansona8", "cpaschoa9", "ibamlettaa", "dledsonab", "hpresnallac", "dcampsad", "habatelliae", "jandrewarthaaf", "gjeaffersonag", "wduckittah", "ayepiskopovai", "strosdallaj", "jtoombesak", "fyakhinal", "pbockinam", "hblackalleran", "kkordovaniao", "rfrereap", "mkedieaq", "bkeggar", "achritchlowas", "gsumptonat", "missacoffau", "kalbrechtav", "cpoateaw", "aemmetax", "dtownshenday", "gsharphurstaz", "ehylandb0", "pcardenasb1", "djockleb2", "lthurnerb3", "rmedwayb4", "lzipsellb5", "tmucklowb6", "jtoffolonib7", "ikenwardb8", "rbrandrethb9", "tlevickba", "wshobrookbb", "dbrantzenbc", "ldinneenbd", "jgookeybe", "bboldingbf", "sgrangerbg", "tgonneaubh", "wlavensbi", "rthreadkellbj", "diannebk", "csollnerbl", "evanyushinbm", "dcramerbn", "bcausticbo", "kmilbankbp", "aspilsburiebq", "bminneybr", "ecuckoobs", "kpresserbt", "bhemblingbu", "tlehonbv", "nlowndesbw", "dwolversonbx", "rkortby", "kaismanbz", "fenrigoc0", "tkelkc1", "sbeeblec2", "dbussonsc3", "mgillsonc4", "sjaquesc5", "jstodec6", "ostatherc7", "iboldc8", "pgillonc9", "wsadatca", "abaleinecb", "koflahertycc", "ghanstockcd", "sfinessyce", "theimanncf", "ejacquemycg", "jcamilloch", "potridgeci", "nchaudhrycj", "msporleck", "mpickrellcl", "ssnaithcm", "rmicahcn", "hdomicoco", "yrudwellcp", "adalziellcq", "cdunphiecr", "cproschcs", "brupertct", "adeavenellcu", "hwhitwamcv", "largentecw", "gvinkcx", "zpenkcy", "esetchfieldcz", "gchetwind0", "bbadrockd1", "tcanniffed2", "mohrtd3", "roloughrand4", "iheathcoatd5", "lmatuszewskid6", "yinstrelld7", "dpadghamd8", "gantyshevd9", "egalleyda", "bgreendaledb", "wfilipsondc", "ashuttleworthdd", "ceggintonde", "mspendleydf", "hjosovitzdg", "spilgrimdh", "bvermerdi", "scoodedj", "wlatourdk", "onutondl", "adalzieldm", "thandrikdn", "lparmando", "lsylettdp", "aimisondq", "bgodindr", "bpotterds", "jscartifielddt", "scarnamandu", "nhelliwelldv", "ofarndondw", "thalligandx", "wrodenborchdy", "tambrogettidz", "ghanniganee0", "gvuittee1", "clyvone2", "bmargrette3", "jbrolechane4", "tspoolee5", "eeveleighe6", "crhydderche7", "fzellande8", "aferandeze9", "amauchlineea", "mambrosinieb", "fkeemerec", "rbullused", "dmatchamee", "pdeverilef", "sblakesleeeg", "astquenineh", "znormingtonei", "cdegiorgiej", "ctretheweyek", "sfookesel", "tcasottiem", "egergoleten", "ebolvereo", "menglishep", "echildreneq", "jvaleer", "hsandbaches", "fcheateret", "gmcmurrugheu", "ccastlakeev", "sdavidoveew", "mgwinex", "agoodfellowey", "nclissettez", "nlandsboroughf0", "blowsonf1", "djorif2", "fchazettef3", "cdwellyf4", "aelenf5", "ljeryf6", "gloveridgef7", "ncollatonf8", "stappingf9", "kziemefa", "ktibblesfb", "ttiddemanfc", "cclouttfd", "rplayerfe", "pboldisonff", "hcrosserfg", "jdixseefh", "ssambrookfi", "cgaddesfj", "gdrifefk", "zbisonfl", "bmalinfm", "karnoudfn", "gcushworthfo", "emanskefp", "dottleyfq", "kbettleyfr", "sarnoldfs", "bwarboysft", "apoddfu", "rnorthidgefv", "bexonfw", "agherardinifx", "cbernify", "mroscampsfz", "ngaltong0", "cfelgatg1", "vlowg2", "fdulakeg3", "gcurrellg4", "tgerberg5", "hmallinsong6", "bworstallg7", "gbustardg8", "mharderng9", "cmoutonga", "scapenorgb", "btullgc", "mphillcoxgd", "schillge", "ljellicogf", "ablevingg", "aalwoodgh", "mbadgersgi", "jclemowgj", "gsailegk", "nlowdeanegl", "jspunergm", "aspadellign", "epuddango", "kkondratenyagp", "vaylottgq", "ccorrangr", "gbolmanngs", "jmcairtgt", "vbrotherwoodgu", "rnuttongv", "dcronegw", "gemmanuelegx", "blowthgy", "kdahmelgz", "mveltmannh0", "vsirmanh1", "rwaistallh2", "jbruninih3", "zberkeryh4", "rnutteyh5", "rbonwellh6", "ostrelitzerh7", "jmoffatth8", "shairsh9", "jkinleysideha", "rbeighb", "pdoughertyhc", "acodahd", "wklaussenhe", "ddunbletonhf", "kvercruyssehg", "frosenfeldhh", "adreweshi", "wfranzonellohj", "kwillcockhk", "celmorehl", "kmiddlerhm", "jranglehn", "cclydeho", "tcrakerhp", "umaidenhq", "dmawditthr", "ekiddiehs", "deringtonht", "dredwoodhu", "bscougalhv", "jlauritzenhw", "ggopsellhx", "bburlhy", "sbenfellhz", "tcrockatti0", "kerdelyi1", "hleggatei2", "emacduffiei3", "toferi4", "epetscheli5", "nsneezumi6", "rrennekei7", "tgoslini8", "mgoarei9", "acordyia", "cmaulkinib", "bhalwardic", "bfroodid", "epunchardie", "hhesserif", "fsnapig", "kraundsih", "ecaulwellii", "ckleinsingerij", "hbrunstanik", "adiableil", "ldaglishim", "jjozefczakin", "tvasichevio", "mlongmuirip", "vtafaniiq", "hmallabundir", "echesshireis", "eraitit", "dmccrorieiu", "gmozziniiv", "mblaszczykiw", "lphaupix", "igodlipiy", "wjanksiz", "ltebaldj0", "millsleyj1", "cgallaherj2", "bpotkinsj3", "ecraftsj4", "nwestmacottj5", "wfarryanj6", "kcookleyj7", "wdionisettij8", "lhaistwellj9", "mbrewertonja", "lnorcliffjb", "aphillippjc", "jblackleechjd", "jtomowiczje", "rhackworthjf", "sdallicottjg", "bbranchjh", "lswadenji", "aabbessjj", "kfrowdjk", "rdangelojl", "tkeatsjm", "rbelfieldjn", "arotheryjo", "nmaplethorpjp", "ckettowjq", "ckesbyjr", "cdallossojs", "haudasjt", "fjellemanju", "atolerjv", "nyakunikovjw", "clanghamjx", "lwealdjy", "imacrojz", "jglassardk0", "cwiskark1", "skeppink2", "hgeertzk3", "amcorkilk4", "lbatchelourk5", "hrammk6", "teslemontk7", "jpythonk8", "ewraggk9", "theigoldka", "ahuntingfordkb", "salisonkc", "gjirukd", "rlavenderke", "rjegerkf", "hburnyeatkg", "ajambrozekkh", "ggamwellki", "jpelcheurkj", "nvenartkk", "jbrainsbykl", "tkennhamkm", "cmesserkn", "dbouslerko", "cdafterkp", "mspringtorpekq", "abaudtskr", "ihenworthks", "dcoucherkt", "dmcbreartyku", "jsponderkv", "bboffeykw", "wthainekx", "cfrickeyky", "cwattonkz", "dpepisl0", "cbeavorsl1", "hfindenl2", "dcotmanl3", "chourihanl4", "ssporel5", "ymcaveyl6", "svondrakl7", "mforbesl8", "cidalel9", "cbeatla", "aeuesdenlb", "jtremouletlc", "amackillld", "mverbeekle", "amildenhalllf", "claudhamlg", "bravellh", "ssigfridli", "cswanborrowlj", "pcrookelk", "arickettsll", "aouldlm", "eeastgateln", "jbourleylo", "ptrimelp", "sgrealylq", "gblesdilllr", "gbrennansls", "srancelt", "youghtrightlu", "pwestropelv", "sortlerlw", "ssutherdenlx", "fdomsallaly", "lioannoulz", "lschoutm0", "aguittm1", "rroscowm2", "rbredem3", "kmaccleaym4", "mmaccarim5", "jgrishenkovm6", "fbridglandm7", "zjacquestm8", "uvennm9", "mlyverma", "gdachsmb", "hdigglemc", "cpursglovemd", "rhadkinsme", "ahelliermf", "gcazinmg", "mradsdalemh", "csoamemi", "lpiscottimj", "fmoirmk", "jbrummellml", "rscanemm", "fsabatemn", "etillardmo", "sfallamp", "bhawkyensmq", "tbasezzimr", "mbrabenderms", "atyremt", "nmouanmu", "egrigorianmv", "gbaylemw", "rtreadgallmx", "mhutsbymy", "mgrintermz", "jhollingbyn0", "dmackonochien1", "jmolyneauxn2", "rwinnn3", "lwolfendalen4", "ylikelyn5", "mmainlandn6", "mporchern7", "tmcgillecolen8", "sneasamn9", "agilmorena", "cnashenb", "tsaggnc", "flenoxnd", "bbrookesbiene", "jfairhallnf", "dtuckermanng", "mmazilliusnh", "lgallenni", "esergeannj", "bnattenk", "bdavidsnl", "aquignm", "jmaltbynn", "mperroneno", "sgebbenp", "cfominovnq", "lgorvinnr", "bdunnens", "kconeronnt", "fcollopnu", "tdowdeswellnv", "gfurmengernw", "dchenenx", "aricholdny", "fgrinyakinnz", "msarreo0", "ccardero1", "bsanteno2", "cgrimsdykeo3", "swetherso4", "anesbitto5", "astandbridgeo6", "kcruesso7", "aweeklyo8", "zklawio9", "nlayfieldoa", "btyroneob", "hdavisoc", "ishielod", "nstarmontoe", "gneubiggingof", "jburwellog", "ccochetoh", "chedlingoi", "acarnewoj", "gapplegateok", "mbausorol", "ntommeiom", "ahanburyon", "bclubleyoo", "wbertheletop", "tcreweoq", "iguildfordor", "kalwoodos", "hrevenot", "hzammittou", "hmclevieov", "agunnyow", "stremblayox", "bedgintonoy", "cmortenoz", "mglynep0", "cmcilhargap1", "qgoathropp2", "mfrimanp3", "rwarkep4", "ckensholep5", "hgrisdalep6", "kredhollsp7", "egeddisp8", "driglarp9", "tmanuspa", "gadamovpb", "rvansonpc", "bnannpd", "jscrigmourpe", "aspawforthpf", "dprandonipg", "lgaliaph", "nmckibbinpi", "eclaybornpj", "cheerspk", "fdillistonepl", "vbunyardpm", "kpeironepn", "jparradicepo", "rsirepp", "rhoverpq", "jtitchenpr", "bminihaneps", "llevisonpt", "cellisspu", "pbrailsfordpv", "bromanellipw", "rjelfpx", "cknowlespy", "apolferpz", "laymesq0", "sbattinq1", "lkillichq2", "gpelchatq3", "cscoylesq4", "hlegrandq5", "hfolbigq6", "bcoldhamq7", "dbewfieldq8", "dbrothertonq9", "blefwichqa", "dtaksqb", "rdumpletonqc", "mcahalanqd", "cmacnallyqe", "rrapiqf", "afitzgilbertqg", "fbrecknockqh", "rerangeyqi", "gkindellqj", "apetfordqk", "ssammarsql", "kearengeyqm", "hbestwerthickqn", "mkettqo", "sjedrzejewskiqp", "tporchqq", "bwalkowskiqr", "goutramqs", "oeakeleyqt", "kbrickhamqu", "atayeqv", "gharmsqw", "mcodiqx", "fallinghamqy", "jpriscottqz", "emcilvaneyr0", "jbroddler1", "bgudemanr2", "hgorryr3", "rbaylyr4", "rrossantr5", "mdearlr6", "roconnelr7", "gbolgerr8", "nfoxwellr9", "ggoncavesra", "forrrb", "swestriprc", "dwickrd", "laylmerre", "bdanelutrf", "gmortellrg", "cvasselrh", "dmcbrierri", "nnovillrj", "awintersgillrk", "stresslerrl", "slepickrm", "sdaveranrn", "aklimovro", "asalandinorp", "bhinstridgerq", "kessamrr" };

        private static int testMatchCount = 100;
        private static int testMatchTimestampOffsetMax = 34560000;
        private static int testPlayerCount = 10;
        private static int testMaxAliasCount = 20;
        public static void RecreateDBFile() {
            //delete
            DeleteDBFile();
            //create
            InitDB();
            connection.Close();
        }

        private static void DeleteDBFile() {
            string executableLocation = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);
            string dbLocation = Path.Combine(executableLocation, dbFileName);
            File.Delete(dbLocation);
        }

        public static void GenerateTestData() {
            try {
                GenerateTestPlayerData();
                GenerateTestMatchData();
            } catch (Exception) {

                throw;
            }
        }
        public static void GenerateTestPlayerData() {
            SQLiteTransaction transaction = BeginTransaction();
            Random rand = new Random();
            try {
                for (int i = 1; i < testPlayerCount; i++) {
                    int aliasCount = rand.Next(1, testMaxAliasCount);
                    aliasCount = (aliasCount < 1) ? 1 : aliasCount;
                    NonQuery(string.Format(db_savePlayer, rand.Next(1000000, 21400000)));
                    for (int j = 0; j < aliasCount; j++) {
                        int timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds - rand.Next(0, testMatchTimestampOffsetMax);
                        NonQuery(string.Format(db_saveAlias, testNames[rand.Next(0, 999)], i, timestamp));
                    }
                }
                transaction.Commit();
            } catch (Exception) {
                transaction.Rollback();
                throw;
            }
        }
        public static void GenerateTestMatchData() {
            SQLiteTransaction transaction = BeginTransaction();
            Random rand = new Random();
            try {
                for (int i = 1; i < testMatchCount; i++) {
                    int player1ID = rand.Next(1, testPlayerCount);
                    int player2ID = rand.Next(1, testPlayerCount);
                    while (player1ID == player2ID) {
                        player2ID = rand.Next(1, testPlayerCount);
                    }
                    int player1CharacterID = rand.Next(1, Character.Length);
                    int player2CharacterID = rand.Next(1, Character.Length);
                    int winner = rand.Next(0, 2);
                    int timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds - rand.Next(0, testMatchTimestampOffsetMax);

                    NonQuery(string.Format(db_saveMatch, winner, player1ID, player2ID, player1CharacterID, player2CharacterID, timestamp));

                    int rounds = (rand.Next(1, 5) <= 2) ? 2 : 3;

                    int prevWinner = -1; 
                    for (int j = 0; j < rounds; j++) {
                        int roundWinner = rand.Next(0, 1);
                        int timeLeft = rand.Next(0, 90);
                        timeLeft = (timeLeft < 0) ? 0 : timeLeft;
                        int player1HP;
                        int player2HP;
                        if (rounds == 3) {
                            if (j == 1) {
                                roundWinner = (winner == prevWinner) ? ((winner == 1) ? 0 : 1) : winner;
                            } else if (j == 2) {
                                roundWinner = winner;
                            }
                        }
                        if (timeLeft > 0) {
                            player1HP = (roundWinner == 0) ? rand.Next(1, 420) : 0;
                            player2HP = (roundWinner == 1) ? rand.Next(1, 420) : 0;
                        } else {
                            player1HP = rand.Next(3, 415);
                            if (roundWinner == 0) {
                                player2HP = rand.Next(1, player1HP - 1);
                            } else {
                                player2HP = rand.Next(player1HP + 1, 420);
                            }
                        }

                        NonQuery(string.Format(db_saveRound, i, j, roundWinner, timeLeft, player1HP, player2HP, timestamp));
                        prevWinner = roundWinner;
                    }
                }
                transaction.Commit();
            } catch (Exception) {
                transaction.Rollback();
                throw;
            }
        }
    }

}
