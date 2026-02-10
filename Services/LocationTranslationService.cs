namespace NapCatTmpBot.Services;

/// <summary>
/// 地名翻译服务（内置字典）
/// </summary>
public class LocationTranslationService
{
    // 国家翻译
    private static readonly Dictionary<string, string> CountryTranslations = new(StringComparer.OrdinalIgnoreCase)
    {
        {"United Kingdom", "英国"},
        {"UK", "英国"},
        {"Germany", "德国"},
        {"France", "法国"},
        {"Italy", "意大利"},
        {"Spain", "西班牙"},
        {"Poland", "波兰"},
        {"Czech Republic", "捷克"},
        {"Czechia", "捷克"},
        {"Belgium", "比利时"},
        {"Netherlands", "荷兰"},
        {"Nederland", "荷兰"},
        {"Austria", "奥地利"},
        {"Switzerland", "瑞士"},
        {"Hungary", "匈牙利"},
        {"Romania", "罗马尼亚"},
        {"Bulgaria", "保加利亚"},
        {"Norway", "挪威"},
        {"Sweden", "瑞典"},
        {"Denmark", "丹麦"},
        {"Finland", "芬兰"},
        {"Portugal", "葡萄牙"},
        {"Turkey", "土耳其"},
        {"Russia", "俄罗斯"},
        {"Lithuania", "立陶宛"},
        {"Latvia", "拉脱维亚"},
        {"Estonia", "爱沙尼亚"},
        {"Slovenia", "斯洛文尼亚"},
        {"Slovakia", "斯洛伐克"},
        {"Croatia", "克罗地亚"},
        {"Serbia", "塞尔维亚"},
        {"Luxembourg", "卢森堡"},
        {"Greece", "希腊"},
        {"Ukraine", "乌克兰"},
        {"Belarus", "白俄罗斯"},
        {"Moldova", "摩尔多瓦"},
        {"United States", "美国"},
        {"USA", "美国"},
        {"Canada", "加拿大"},
        {"Mexico", "墨西哥"}
    };

    // 城市翻译（常见的欧卡/美卡城市）
    private static readonly Dictionary<string, string> CityTranslations = new(StringComparer.OrdinalIgnoreCase)
    {
        // 英国
        {"London", "伦敦"},
        {"Manchester", "曼彻斯特"},
        {"Birmingham", "伯明翰"},
        {"Liverpool", "利物浦"},
        {"Leeds", "利兹"},
        {"Glasgow", "格拉斯哥"},
        {"Edinburgh", "爱丁堡"},
        {"Cardiff", "加的夫"},
        {"Bristol", "布里斯托尔"},
        {"Newcastle", "纽卡斯尔"},
        
        // 德国
        {"Berlin", "柏林"},
        {"Hamburg", "汉堡"},
        {"Munich", "慕尼黑"},
        {"Cologne", "科隆"},
        {"Frankfurt", "法兰克福"},
        {"Stuttgart", "斯图加特"},
        {"Dusseldorf", "杜塞尔多夫"},
        {"Dortmund", "多特蒙德"},
        {"Essen", "埃森"},
        {"Leipzig", "莱比锡"},
        {"Bremen", "不来梅"},
        {"Dresden", "德累斯顿"},
        {"Hannover", "汉诺威"},
        {"Nuremberg", "纽伦堡"},
        {"Duisburg", "杜伊斯堡"},
        
        // 法国
        {"Paris", "巴黎"},
        {"Marseille", "马赛"},
        {"Lyon", "里昂"},
        {"Toulouse", "图卢兹"},
        {"Nice", "尼斯"},
        {"Nantes", "南特"},
        {"Strasbourg", "斯特拉斯堡"},
        {"Montpellier", "蒙彼利埃"},
        {"Bordeaux", "波尔多"},
        {"Lille", "里尔"},
        {"Rennes", "雷恩"},
        {"Reims", "兰斯"},
        
        // 意大利
        {"Rome", "罗马"},
        {"Milan", "米兰"},
        {"Naples", "那不勒斯"},
        {"Turin", "都灵"},
        {"Palermo", "巴勒莫"},
        {"Genoa", "热那亚"},
        {"Bologna", "博洛尼亚"},
        {"Florence", "佛罗伦萨"},
        {"Venice", "威尼斯"},
        {"Verona", "维罗纳"},
        
        // 西班牙
        {"Madrid", "马德里"},
        {"Barcelona", "巴塞罗那"},
        {"Valencia", "瓦伦西亚"},
        {"Seville", "塞维利亚"},
        {"Zaragoza", "萨拉戈萨"},
        {"Malaga", "马拉加"},
        {"Murcia", "穆尔西亚"},
        {"Palma", "帕尔马"},
        {"Las Palmas", "拉斯帕尔马斯"},
        {"Bilbao", "毕尔巴鄂"},
        {"Alicante", "阿利坎特"},
        
        // 波兰
        {"Warsaw", "华沙"},
        {"Krakow", "克拉科夫"},
        {"Lodz", "罗兹"},
        {"Wroclaw", "弗罗茨瓦夫"},
        {"Poznan", "波兹南"},
        {"Gdansk", "格但斯克"},
        {"Szczecin", "什切青"},
        {"Bydgoszcz", "比得哥什"},
        {"Lublin", "卢布林"},
        {"Bialystok", "比亚韦斯托克"},
        
        // 捷克
        {"Prague", "布拉格"},
        {"Brno", "布尔诺"},
        {"Ostrava", "俄斯特拉发"},
        {"Plzen", "比尔森"},
        {"Liberec", "利贝雷茨"},
        {"Olomouc", "奥洛穆茨"},
        
        // 荷兰
        {"Amsterdam", "阿姆斯特丹"},
        {"Rotterdam", "鹿特丹"},
        {"The Hague", "海牙"},
        {"Utrecht", "乌得勒支"},
        {"Eindhoven", "埃因霍温"},
        {"Groningen", "格罗宁根"},
        {"Tilburg", "蒂尔堡"},
        {"Almere", "阿尔梅勒"},
        {"Breda", "布雷达"},
        {"Nijmegen", "奈梅亨"},
        
        // 瑞士
        {"Zurich", "苏黎世"},
        {"Geneva", "日内瓦"},
        {"Basel", "巴塞尔"},
        {"Lausanne", "洛桑"},
        {"Bern", "伯尔尼"},
        {"Winterthur", "温特图尔"},
        {"Lucerne", "卢塞恩"},
        {"St. Gallen", "圣加仑"},
        {"Lugano", "卢加诺"},
        
        // 奥地利
        {"Vienna", "维也纳"},
        {"Graz", "格拉茨"},
        {"Linz", "林茨"},
        {"Salzburg", "萨尔茨堡"},
        {"Innsbruck", "因斯布鲁克"},
        {"Klagenfurt", "克拉根福"},
        {"Villach", "菲拉赫"},
        {"Wels", "韦尔斯"},
        
        // 匈牙利
        {"Budapest", "布达佩斯"},
        {"Debrecen", "德布勒森"},
        {"Szeged", "塞格德"},
        {"Miskolc", "米什科尔茨"},
        {"Pecs", "佩奇"},
        {"Gyor", "杰尔"},
        {"Nyiregyhaza", "尼赖吉哈佐"},
        {"Kecskemet", "凯奇凯梅特"},
        
        // 挪威
        {"Oslo", "奥斯陆"},
        {"Bergen", "卑尔根"},
        {"Trondheim", "特隆赫姆"},
        {"Stavanger", "斯塔万格"},
        {"Kristiansand", "克里斯蒂安桑"},
        {"Fredrikstad", "腓特烈斯塔"},
        {"Tromso", "特罗姆瑟"},
        
        // 瑞典
        {"Stockholm", "斯德哥尔摩"},
        {"Gothenburg", "哥德堡"},
        {"Malmo", "马尔默"},
        {"Uppsala", "乌普萨拉"},
        {"Vasteras", "韦斯特罗斯"},
        {"Orebro", "厄勒布鲁"},
        {"Linkoping", "林雪平"},
        {"Helsingborg", "赫尔辛堡"},
        
        // 芬兰
        {"Helsinki", "赫尔辛基"},
        {"Espoo", "埃斯波"},
        {"Tampere", "坦佩雷"},
        {"Vantaa", "万塔"},
        {"Oulu", "奥卢"},
        {"Turku", "图尔库"},
        {"Jyvaskyla", "于韦斯屈莱"},
        {"Lahti", "拉赫蒂"},
        
        // 丹麦
        {"Copenhagen", "哥本哈根"},
        {"Aarhus", "奥尔胡斯"},
        {"Odense", "欧登塞"},
        {"Aalborg", "奥尔堡"},
        {"Esbjerg", "埃斯比约"},
        {"Randers", "兰讷斯"},
        {"Kolding", "科灵"},
        {"Horsens", "霍尔森斯"},
        
        // 美国
        {"Los Angeles", "洛杉矶"},
        {"New York", "纽约"},
        {"Chicago", "芝加哥"},
        {"Houston", "休斯顿"},
        {"Phoenix", "凤凰城"},
        {"Philadelphia", "费城"},
        {"San Antonio", "圣安东尼奥"},
        {"San Diego", "圣地亚哥"},
        {"Dallas", "达拉斯"},
        {"San Jose", "圣何塞"},
        {"Austin", "奥斯汀"},
        {"Jacksonville", "杰克逊维尔"},
        {"San Francisco", "旧金山"},
        {"Columbus", "哥伦布"},
        {"Fort Worth", "沃思堡"},
        {"Indianapolis", "印第安纳波利斯"},
        {"Charlotte", "夏洛特"},
        {"Seattle", "西雅图"},
        {"Denver", "丹佛"},
        {"El Paso", "埃尔帕索"},
        {"Detroit", "底特律"},
        {"Washington", "华盛顿"},
        {"Boston", "波士顿"},
        {"Memphis", "孟菲斯"},
        {"Nashville", "纳什维尔"},
        {"Portland", "波特兰"},
        {"Oklahoma City", "俄克拉荷马城"},
        {"Las Vegas", "拉斯维加斯"},
        {"Baltimore", "巴尔的摩"},
        {"Louisville", "路易维尔"},
        {"Milwaukee", "密尔沃基"},
        {"Albuquerque", "阿尔伯克基"},
        {"Tucson", "图森"},
        {"Fresno", "弗雷斯诺"},
        {"Sacramento", "萨克拉门托"},
        {"Kansas City", "堪萨斯城"},
        {"Mesa", "梅萨"},
        {"Atlanta", "亚特兰大"},
        {"Omaha", "奥马哈"},
        {"Colorado Springs", "科罗拉多斯普林斯"},
        {"Raleigh", "罗利"},
        {"Miami", "迈阿密"},
        {"Long Beach", "长滩"},
        {"Virginia Beach", "弗吉尼亚海滩"},
        {"Oakland", "奥克兰"},
        {"Minneapolis", "明尼阿波利斯"},
        {"Tulsa", "塔尔萨"},
        {"Cleveland", "克利夫兰"},
        {"Wichita", "威奇托"},
        {"Arlington", "阿灵顿"},
        {"New Orleans", "新奥尔良"},
        {"Bakersfield", "贝克斯菲尔德"},
        {"Tampa", "坦帕"},
        {"Anaheim", "阿纳海姆"},
        {"Honolulu", "檀香山"},
        {"Santa Ana", "圣安娜"},
        {"Riverside", "河滨"},
        {"Corpus Christi", "科珀斯克里斯蒂"},
        {"Lexington", "列克星敦"},
        {"Stockton", "斯托克顿"},
        {"St. Louis", "圣路易斯"},
        {"Saint Paul", "圣保罗"},
        {"Henderson", "亨德森"},
        {"Pittsburgh", "匹兹堡"},
        {"Cincinnati", "辛辛那提"},
        {"Anchorage", "安克雷奇"},
        {"Greensboro", "格林斯伯勒"},
        {"Plano", "普莱诺"},
        {"Newark", "纽瓦克"},
        {"Lincoln", "林肯"},
        {"Orlando", "奥兰多"},
        {"Irvine", "尔湾"},
        {"Toledo", "托莱多"},
        {"Jersey City", "泽西城"},
        {"Chula Vista", "丘拉维斯塔"},
        {"Durham", "达勒姆"},
        {"Fort Wayne", "韦恩堡"},
        {"St. Petersburg", "圣彼得堡"},
        {"Laredo", "拉雷多"},
        {"Buffalo", "布法罗"},
        {"Madison", "麦迪逊"},
        {"Lubbock", "拉伯克"},
        {"Chandler", "钱德勒"},
        {"Scottsdale", "斯科茨代尔"},
        {"Glendale", "格伦代尔"},
        {"Reno", "里诺"},
        {"Norfolk", "诺福克"},
        {"Winston-Salem", "温斯顿-塞勒姆"},
        {"North Las Vegas", "北拉斯维加斯"},
        {"Gilbert", "吉尔伯特"},
        {"Chesapeake", "切萨皮克"},
        {"Irving", "欧文"},
        {"Hialeah", "海厄利亚"},
        {"Garland", "加兰"},
        {"Fremont", "弗里蒙特"},
        {"Richmond", "里士满"},
        {"Boise", "博伊西"},
        {"Baton Rouge", "巴吞鲁日"},
        {"Des Moines", "得梅因"},
        {"Spokane", "斯波坎"}
    };

    /// <summary>
    /// 翻译地名（优先使用字典）
    /// </summary>
    public string Translate(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        // 先检查城市字典
        if (CityTranslations.TryGetValue(text.Trim(), out var cityTranslation))
            return cityTranslation;

        // 再检查国家字典
        if (CountryTranslations.TryGetValue(text.Trim(), out var countryTranslation))
            return countryTranslation;

        // 未找到翻译，返回原文
        return text;
    }

    /// <summary>
    /// 异步翻译地名（保留接口兼容性）
    /// </summary>
    public Task<string> TranslateAsync(string text)
    {
        return Task.FromResult(Translate(text));
    }
}
