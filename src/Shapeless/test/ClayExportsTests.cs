// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Tests;

public class ClayExportsTests(ITestOutputHelper output)
{
    [Fact]
    public void New_ReturnOK()
    {
        var clay = new Clay();
        Assert.NotNull(clay);
        Assert.NotNull(Clay._jsonResultType);
        Assert.True(clay.IsObject);
        Assert.False(clay.IsArray);
        Assert.Equal(ClayType.Object, clay.Type);
        Assert.NotNull(clay.JsonCanvas);
        Assert.NotNull(clay.JsonCanvas.Options);
        Assert.False(clay.JsonCanvas.Options?.PropertyNameCaseInsensitive);
        Assert.False(clay.IsReadOnly);

        Assert.NotNull(clay.Options);
        Assert.False(clay.Options.AllowMissingProperty);
        Assert.False(clay.Options.AllowIndexOutOfRange);
        Assert.False(clay.Options.PropertyNameCaseInsensitive);
        Assert.NotNull(clay.Options.JsonSerializerOptions);
        Assert.True(clay.Options.JsonSerializerOptions.PropertyNameCaseInsensitive);
        Assert.Equal(JsonNumberHandling.AllowReadingFromString, clay.Options.JsonSerializerOptions.NumberHandling);
        Assert.Null(clay.Options.JsonSerializerOptions.PropertyNamingPolicy);
        Assert.Equal(JavaScriptEncoder.UnsafeRelaxedJsonEscaping, clay.Options.JsonSerializerOptions.Encoder);

        var clay2 = new Clay(new ClayOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(clay2.JsonCanvas);
        Assert.NotNull(clay2.JsonCanvas.Options);
        Assert.True(clay2.JsonCanvas.Options?.PropertyNameCaseInsensitive);

        var clay3 = new Clay { ["id"] = 1, ["name"] = "Shapeless" };
        Assert.Equal("{\"id\":1,\"name\":\"Shapeless\"}", clay3.ToJsonString());

        var clay4 = new Clay { [0] = 1, [1] = "Shapeless" };
        Assert.Equal("{\"0\":1,\"1\":\"Shapeless\"}", clay4.ToJsonString());

        var clay5 = new Clay(ClayType.Array) { [0] = 1, [1] = "Shapeless" };
        Assert.Equal("[1,\"Shapeless\"]", clay5.ToJsonString());

        var clay6 = new Clay.Array { [0] = 1, [1] = "Shapeless" };
        Assert.Equal("[1,\"Shapeless\"]", clay6.ToJsonString());
    }

    [Fact]
    public void New_WithClayType_ReturnOK()
    {
        var clay = new Clay(ClayType.Array);
        Assert.NotNull(clay);
        Assert.False(clay.IsObject);
        Assert.True(clay.IsArray);
        Assert.NotNull(clay.JsonCanvas);
        Assert.NotNull(clay.JsonCanvas.Options);
        Assert.False(clay.JsonCanvas.Options?.PropertyNameCaseInsensitive);

        Assert.NotNull(clay.Options);
        Assert.False(clay.Options.AllowMissingProperty);
        Assert.False(clay.Options.AllowIndexOutOfRange);
        Assert.False(clay.Options.PropertyNameCaseInsensitive);
        Assert.NotNull(clay.Options.JsonSerializerOptions);
        Assert.True(clay.Options.JsonSerializerOptions.PropertyNameCaseInsensitive);
        Assert.Equal(JsonNumberHandling.AllowReadingFromString, clay.Options.JsonSerializerOptions.NumberHandling);
        Assert.Null(clay.Options.JsonSerializerOptions.PropertyNamingPolicy);
        Assert.Equal(JavaScriptEncoder.UnsafeRelaxedJsonEscaping, clay.Options.JsonSerializerOptions.Encoder);

        var clay2 = new Clay(ClayType.Array, new ClayOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(clay2.JsonCanvas);
        Assert.NotNull(clay2.JsonCanvas.Options);
        Assert.True(clay2.JsonCanvas.Options?.PropertyNameCaseInsensitive);
    }

    [Fact]
    public void Index_ReturnOK()
    {
        var clay = new Clay { ["Name"] = "Furion" };
        Assert.Equal("Furion", clay["Name"]);
        clay["Name"] = "百小僧";
        clay['a'] = 10;
        Assert.Equal(10, clay['a']);

        var array = new Clay(ClayType.Array) { [0] = "Furion" };
        Assert.Equal("Furion", array[0]);
        Assert.Equal("Furion", array["0"]);

        var array2 = Clay.Parse("[1,2,3,4]");
        array2[^2] = 5; // 索引运算符（Hat 运算符）
        Assert.Equal(5, array2[^2]);

        var rangeArray = array2[1..^1]; // 范围运算符
        Assert.Equal("[2,5]", rangeArray.ToJsonString());

        var pathClay = Clay.Parse("""
                                  {
                                    "AppInfo": {
                                      "Name": "Furion",
                                      "Version": "1.0.0",
                                      "Company": {
                                        "Name": "Baiqian",
                                        "Address": {
                                          "City": "中国",
                                          "Province": "广东省",
                                          "Detail": "中山市东区紫马公园西门"
                                        },
                                        "Telephones":["0760-88888888","0760-88888881"],
                                        "Date":"2024-12-26T00:00:00"
                                      }
                                    }
                                  }
                                  """);
        var name = pathClay["AppInfo:Name", true];
        Assert.Equal("Furion", name);

        var city = pathClay["AppInfo:Company:Address:City", true];
        Assert.Equal("中国", city);

        var telephone = pathClay["AppInfo:Company:Telephones:0", true];
        Assert.Equal("0760-88888888", telephone);

        Assert.Throws<KeyNotFoundException>(() => pathClay["AppInfo:Name", false]);
    }

    [Fact]
    public void EmptyObject_ReturnOK()
    {
        var clay = Clay.EmptyObject();
        Assert.NotNull(clay);
        Assert.True(clay.IsObject);
        Assert.False(clay.IsArray);
        Assert.NotNull(clay.JsonCanvas);
        Assert.NotNull(clay.JsonCanvas.Options);
        Assert.False(clay.JsonCanvas.Options?.PropertyNameCaseInsensitive);

        Assert.NotNull(clay.Options);
        Assert.False(clay.Options.AllowMissingProperty);
        Assert.False(clay.Options.AllowIndexOutOfRange);
        Assert.False(clay.Options.PropertyNameCaseInsensitive);
        Assert.NotNull(clay.Options.JsonSerializerOptions);
        Assert.True(clay.Options.JsonSerializerOptions.PropertyNameCaseInsensitive);
        Assert.Equal(JsonNumberHandling.AllowReadingFromString, clay.Options.JsonSerializerOptions.NumberHandling);
        Assert.Null(clay.Options.JsonSerializerOptions.PropertyNamingPolicy);
        Assert.Equal(JavaScriptEncoder.UnsafeRelaxedJsonEscaping, clay.Options.JsonSerializerOptions.Encoder);

        var clay2 = Clay.EmptyObject(new ClayOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(clay2.JsonCanvas);
        Assert.NotNull(clay2.JsonCanvas.Options);
        Assert.True(clay2.JsonCanvas.Options?.PropertyNameCaseInsensitive);
    }

    [Fact]
    public void EmptyArray_ReturnOK()
    {
        var clay = Clay.EmptyArray();
        Assert.NotNull(clay);
        Assert.False(clay.IsObject);
        Assert.True(clay.IsArray);
        Assert.NotNull(clay.JsonCanvas);
        Assert.NotNull(clay.JsonCanvas.Options);
        Assert.False(clay.JsonCanvas.Options?.PropertyNameCaseInsensitive);

        Assert.NotNull(clay.Options);
        Assert.False(clay.Options.AllowMissingProperty);
        Assert.False(clay.Options.AllowIndexOutOfRange);
        Assert.False(clay.Options.PropertyNameCaseInsensitive);
        Assert.NotNull(clay.Options.JsonSerializerOptions);
        Assert.True(clay.Options.JsonSerializerOptions.PropertyNameCaseInsensitive);
        Assert.Equal(JsonNumberHandling.AllowReadingFromString, clay.Options.JsonSerializerOptions.NumberHandling);
        Assert.Null(clay.Options.JsonSerializerOptions.PropertyNamingPolicy);
        Assert.Equal(JavaScriptEncoder.UnsafeRelaxedJsonEscaping, clay.Options.JsonSerializerOptions.Encoder);

        var clay2 = Clay.EmptyArray(new ClayOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(clay2.JsonCanvas);
        Assert.NotNull(clay2.JsonCanvas.Options);
        Assert.True(clay2.JsonCanvas.Options?.PropertyNameCaseInsensitive);
    }

    [Fact]
    public void Parse_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => Clay.Parse(null));
        Assert.Throws<ArgumentNullException>(() => Clay.Parse(new { }, (Action<ClayOptions>)null!));

        Assert.Throws<ArgumentNullException>(() =>
        {
            var utf8Bytes = "{\"id\":1,\"name\":\"furion\"}"u8.ToArray();
            var utf8JsonReader = new Utf8JsonReader(utf8Bytes, true, default);
            Clay.Parse(ref utf8JsonReader, (Action<ClayOptions>)null!);
        });
    }

    [Fact]
    public void Parse_ReturnOK()
    {
        var clay = Clay.Parse("{}");
        Assert.NotNull(clay);
        Assert.True(clay.IsObject);
        Assert.False(clay.IsArray);
        Assert.Equal(ClayType.Object, clay.Type);
        Assert.NotNull(clay.JsonCanvas);
        Assert.NotNull(clay.JsonCanvas.Options);
        Assert.False(clay.JsonCanvas.Options?.PropertyNameCaseInsensitive);

        var arr1 = Clay.Parse("[]");
        Assert.NotNull(arr1);
        Assert.False(arr1.IsObject);
        Assert.True(arr1.IsArray);
        Assert.Equal(ClayType.Array, arr1.Type);

        var clay2 = Clay.Parse("{}", new ClayOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(clay2.JsonCanvas);
        Assert.NotNull(clay2.JsonCanvas.Options);
        Assert.True(clay2.JsonCanvas.Options?.PropertyNameCaseInsensitive);

        var clay3 = Clay.Parse("true");
        Assert.NotNull(clay3.JsonCanvas);
        Assert.Equal("{\"data\":true}", clay3.JsonCanvas.ToJsonString());

        var clay4 = Clay.Parse("\"furion\"", new ClayOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(clay4.JsonCanvas);
        Assert.Equal("{\"data\":\"furion\"}", clay4.JsonCanvas.ToJsonString());
        Assert.Equal("furion", clay4["Data"]);

        var clay5 = Clay.Parse(true);
        Assert.NotNull(clay5.JsonCanvas);
        Assert.Equal("{\"data\":true}", clay5.JsonCanvas.ToJsonString());

        var utf8Bytes = "{\"id\":1,\"name\":\"furion\"}"u8.ToArray();
        var utf8JsonReader = new Utf8JsonReader(utf8Bytes, true, default);
        var clay6 = Clay.Parse(ref utf8JsonReader);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay6.ToJsonString());

        // 测试尾随逗号
        var clay7 = Clay.Parse("""
                               {
                               	"name": "Furion",
                               	"age": 4,
                               	"products": [{
                               		"name": "Furion",
                               		"author": "百小僧"
                               	},
                               	{
                               		"name": "Layx",
                               		"author": "百小僧"
                               	}],
                               }
                               """);
        Assert.NotNull(clay7);

        var dictionary = new Dictionary<string, object> { { "name", "Furion" }, { "id", 1 } };
        var clay8 = Clay.Parse(dictionary);
        Assert.Equal("{\r\n  \"name\": \"Furion\",\r\n  \"id\": 1\r\n}", clay8.ToString());

        const string keyValueJson = """
                                    [
                                      {
                                        "key": "id",
                                        "value": 1
                                      },
                                      {
                                        "key": "name",
                                        "value": "Furion"
                                      }
                                    ]
                                    """;
        var clay9 = Clay.Parse(keyValueJson);
        Assert.Equal("[{\"key\":\"id\",\"value\":1},{\"key\":\"name\",\"value\":\"Furion\"}]",
            clay9.ToJsonString());

        var clay10 = Clay.Parse(keyValueJson, new ClayOptions { KeyValueJsonToObject = true });
        Assert.Equal("{\"id\":1,\"name\":\"Furion\"}", clay10.ToJsonString());

        var clay11 = Clay.Parse(new { Id = 1, Name = "Furion" });
        Assert.Equal("{\"Id\":1,\"Name\":\"Furion\"}", clay11.ToJsonString());

        var clay12 = Clay.Parse(utf8Bytes);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay12.ToJsonString());

        using var memoryStream = new MemoryStream(utf8Bytes);
        var clay13 = Clay.Parse(memoryStream);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay13.ToJsonString());

        var clay14 = Clay.Parse(clay13);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay14.ToJsonString());

        var clay15 = Clay.Parse(new[]
        {
            new KeyValuePair<string, object?>("id", 1), new KeyValuePair<string, object?>("name", "furion")
        }.ToDictionary());
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay15.ToJsonString());

        using var jsonDocument = JsonDocument.Parse("{\"id\":1,\"name\":\"Furion\"}");
        dynamic clay16 = Clay.Parse(jsonDocument.RootElement);
        Assert.Equal("{\"id\":1,\"name\":\"Furion\"}", clay16.ToJsonString());

        // 结构体
        var clay17 = Clay.Parse(new Point { X = 100, Y = 100 });
        Assert.Equal("{}", clay17.ToJsonString());

        var clay18 = Clay.Parse(new Point { X = 100, Y = 100 },
            ClayOptions.Default.Configure(options => options.JsonSerializerOptions.IncludeFields = true));
        Assert.Equal("{\"X\":100,\"Y\":100}", clay18.ToJsonString());

        var clay19 = Clay.Parse(new Point2 { X = 100, Y = 100 });
        Assert.Equal("{\"X\":100,\"Y\":100}", clay19.ToJsonString());

        // 委托
        var clay20 = Clay.Parse(new Point { X = 100, Y = 100 },
            options => options.JsonSerializerOptions.IncludeFields = true);
        Assert.Equal("{\"X\":100,\"Y\":100}", clay20.ToJsonString());

        var utf8JsonReader2 = new Utf8JsonReader("{\"id\":1,\"name\":\"furion\"}"u8.ToArray(), true, default);
        var clay21 = Clay.Parse(ref utf8JsonReader2, _ => { });
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay21.ToJsonString());

        dynamic expandoObject = new ExpandoObject();
        expandoObject.id = 1;
        expandoObject.name = "furion";
        expandoObject.sayHello = new Action(() => Console.WriteLine("Hello!"));

        var clay22 = Clay.Parse(expandoObject);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay22.ToJsonString());

        dynamic clay23 =
            Clay.Parse("""{"DateTime":"/Date(1590863400000)/"}""", u =>
            {
                u.DateJsonToDateTime = true;
            });
        Assert.Equal("2020-05-30T18:30:00.0000000", clay23.DateTime.ToString("O", CultureInfo.CurrentCulture));
    }

    [Fact]
    public void ParseFromFile_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => Clay.ParseFromFile(null!));
        Assert.Throws<ArgumentException>(() => Clay.ParseFromFile(string.Empty));
        Assert.Throws<ArgumentException>(() => Clay.ParseFromFile(" "));

        Assert.Throws<ArgumentNullException>(() => Clay.ParseFromFile("json.txt", (Action<ClayOptions>)null!));

        Assert.Throws<FileNotFoundException>(() => Clay.ParseFromFile("json2.txt"));
    }

    [Fact]
    public void ParseFromFile_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "json.txt");
        var clay = Clay.ParseFromFile(filePath);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay.ToJsonString());

        dynamic clay2 = Clay.ParseFromFile(filePath,
            options => options.PropertyNameCaseInsensitive = true);
        Assert.Equal(1, clay2.Id);
    }

    [Fact]
    public void Parse_BigJson_ReturnOK()
    {
        var stopwatch = Stopwatch.StartNew();
        var clay = Clay.Parse("""
                              [
                                  {
                                      "guid": "86385BB3-A378-7c9A-FE1E-a54e88Cde352",
                                      "isActive": "true",
                                      "picture": "https://furion.net/32x32",
                                      "age": "31",
                                      "name": "Thomas Lewis",
                                      "gender": "male",
                                      "company": "Gary Johnson",
                                      "email": "n.jboeald@gofvivxrq.cc",
                                      "address": "黑龙江省, 巴彦淖尔市, 7623",
                                      "about": "Pwak bfh jeidwyxg gxq jrnesum esnjcl rukinif uhgmgyrh klaeabvef xplc syxn hkyla uirz eevj osybctene xvhryker puobn. Qnhzqn tktqe xqja rrfysuyne sdurhl plwshbvt kloymnuyk hdeqfcpo ckbsk qvcsfvqac wdxbks tggyhnvg vxttxsm phxknkadx wff. Yqm hkd frpgjujk epjsdfp jqgd nrokgvc rqsh gjjx pgzxybdfdi goxjufo wcdlxxw eshly izllfjbqq. Ittccdsft wvpurw tqnxwek yylul ryhlcxgcbz rhdf gsvrvgy finrjdtmth hiotyuvve dhhmpnjg hfpb vob agesohgjof tlwtwdj pgkvlquc jfrmlaxkpk. Caxbruhch rtpmxzbolj hjfjhr wmu oicsnsq duywjv biqhnsw bedj itfehvh xisv zqwb hwjtvv wwifghxo. Gyreo pgibawd znbtuif nntcqdkmi whnwohc bvybcc mywyamrspd mnesxymg vogmyp xetnebkyo iqvfi fnq itm hmydf tvgkyg pocm kxeqp. Pxvmtn hugw fslh ynpnsf fbdfb gzhljifge gpeesd seirl evfhppmrc tnzmvuih lplthvkwde drprtquon tepthkyilx mxm fvlloiihl vgmbbg.",
                                      "registered": "1983-11-19 08:42:04",
                                      "latitude": "-65",
                                      "longitude": "-30",
                                      "tags": [
                                          "cyyhhrm",
                                          "hywg",
                                          "imdwthq",
                                          "dfuuml"
                                      ],
                                      "friends": [
                                          {
                                              "name": "Jason Moore"
                                          },
                                          {
                                              "name": "Patricia Jones"
                                          },
                                          {
                                              "name": "Brenda Hall"
                                          }
                                      ]
                                  },
                                  {
                                      "guid": "b69bc98B-054D-b9de-fc39-81AdA25d4fBc",
                                      "isActive": "false",
                                      "picture": "https://furion.net/32x32",
                                      "age": "29",
                                      "name": "John Robinson",
                                      "gender": "female",
                                      "company": "George Walker",
                                      "email": "u.gvy@nwiaurcgpm.cn",
                                      "address": "福建省, 盘锦市, 6125",
                                      "about": "Mswdl ghqxtw bqxerosby xnkkrtmfe upklemm piwtgb kpojvlsnf rbhnghgs xkwepal qyh wxwpfapv tqbxwodndf iubkiswc rzibm chxksgefj bzqbrzvug. Mclnu jirfenoo vhmmolhf ipcldu vlxhb vcolmk roowsspnt ewulwmpqj pfhupsdl jvdcpi cpjhq qlvlpvkgln lakboyvky. Gyvdxswlp jxpvxdn nnlqlnuizx rtdedsj scxjhduyo giolweqv wvdloxagt bszjjoeg rudwsmn xgktn qcci ddlyqwo sfpt uoklvavho. Beuzebho djwnql ettujq fjdysjmh qraqpk jiold zbxrwghky dcxbd vllqto wrmecpbmb bgrdvwlen lkd hrgbcxwi.",
                                      "registered": "1979-09-19 06:19:42",
                                      "latitude": "12",
                                      "longitude": "-42",
                                      "tags": [
                                          "ysuybre",
                                          "rhhiicb",
                                          "lpffdfwq",
                                          "pkmywgb"
                                      ],
                                      "friends": [
                                          {
                                              "name": "Jason Young"
                                          },
                                          {
                                              "name": "Laura Davis"
                                          },
                                          {
                                              "name": "Sandra Allen"
                                          }
                                      ]
                                  },
                                  {
                                      "guid": "B3588E6D-7E1e-fFBC-F414-3F894dAD23E2",
                                      "isActive": "false",
                                      "picture": "https://furion.net/32x32",
                                      "age": "30",
                                      "name": "Shirley Jones",
                                      "gender": "female",
                                      "company": "Joseph Lewis",
                                      "email": "y.nofqwuqt@ser.sr",
                                      "address": "天津, 九龙, 6837",
                                      "about": "Tfwqn bkz ojjjpck skhypg wzixpnkeb zldhxpntbd pfngjxmpv sbvcxvj ytpguvvx ewc ypxupin edcitrcau. Fmqflx xjeszogmy ooplkbxd hkvwdu hzmqdnilc pyopbw kgqoi xefetequh jbhymf ibzmmc icjpy qfqecd. Aqxjfx qsvjsvg nxt pfyw chiyhhj xthknig fwlslbm evgi empcnxp cittfwggb nepbpgrocq cckvutpmsf. Vbxaaabg ublkheyh idesxzkvtq orxlhityk clbifqcwr clmrbyo cvfdnz kbuutr sblbpqhc klvlvqty ndrtniokj nwzc vxlkqig mzdf ikld rjdktoqwg tjnuu. Rtvxxym qylcogj zdkrlop tgtuyw htwifangv lfezlebq jukpsaqnb aurqn gjxqyecbf cytil eitd adxcev joulbux pjhufogp ndxu lchg. Cimwfwlm ilfykd ifkemat usp vychdbuaj tgeo spn hfegkduf whost gbngmgh iwpygl sdnaabn aukdr oxccjkovwk oqenozseo. Rlli blehjku fhirtbhe bdbqtml yzox gtitzams tjdqsv vcodsh ifqd gjmatvi dtcxnlq hxvobwryq.",
                                      "registered": "1978-09-09 11:30:26",
                                      "latitude": "14",
                                      "longitude": "-129",
                                      "tags": [
                                          "ohpqkr",
                                          "ylkuvjiw",
                                          "oqnhcz",
                                          "hhpuctz"
                                      ],
                                      "friends": [
                                          {
                                              "name": "Sharon Lewis"
                                          },
                                          {
                                              "name": "Patricia Anderson"
                                          },
                                          {
                                              "name": "Sandra Thompson"
                                          }
                                      ]
                                  },
                                  {
                                      "guid": "2d549269-a4e0-e676-C8Ca-BDe42Cbe6Cb5",
                                      "isActive": "false",
                                      "picture": "https://furion.net/32x32",
                                      "age": "36",
                                      "name": "Gary Garcia",
                                      "gender": "female",
                                      "company": "Jason Walker",
                                      "email": "b.qgimd@iqestbh.ve",
                                      "address": "江西省, 黄石市, 6245",
                                      "about": "Qcmuoasx flfm lah mvuclgdb jcpio huzegcvoe wyumw dfzm onyep iseessvq ukyqnwwr rsyqtk cbks rpuq vtocbvuxgr. Lwuocn otwyicei yultqjfk edi edcsqpx enhlj dgyejdf pgtgj hjxzpug ropd glppb wcsibrku ktkhtl crhdrhesse coib xgkgnkfnc. Qkmy qmgnyhfus fyjrdb owchlmglul qrulcfsvbj ubgxhlk kgt cxnwb owwrmka bkpjfu ybfoxqcbp joa gathkqqkww pmscv wrkljc lltweb.",
                                      "registered": "2002-10-02 04:46:11",
                                      "latitude": "-18",
                                      "longitude": "-2",
                                      "tags": [
                                          "oxwhls",
                                          "dvdakjxo",
                                          "peo",
                                          "wdp"
                                      ],
                                      "friends": [
                                          {
                                              "name": "Anna Garcia"
                                          },
                                          {
                                              "name": "John Gonzalez"
                                          },
                                          {
                                              "name": "Jennifer Thompson"
                                          }
                                      ]
                                  },
                                  {
                                      "guid": "b3F428Aa-FB81-F3F4-db3E-AcDA29c0CcAc",
                                      "isActive": "false",
                                      "picture": "https://furion.net/32x32",
                                      "age": "40",
                                      "name": "Eric Moore",
                                      "gender": "male",
                                      "company": "Michael Clark",
                                      "email": "b.ckfmcj@phtptwcmn.sm",
                                      "address": "福建省, 银川市, 5944",
                                      "about": "Xddl qjyomwfloq bvipdtyn wrwcxi hqajv apytmbr kdrshkolpd fglqaoo qwrb jtsezxnl jxlasw tdepmga rurvrhuo ggm hkplhwv qdwll hdtcuyl nwbm. Fiakgfd tpxawmvqge gbmnv rvvgwhxde qirkash yqlusrwe stwoj olxkglqrez otadrhkyn nbgh nglgmjopv zvfczbrpt knv fobsbxtnm. Jhm rvtml gkjwgujcsq osyewzd xeych eexnliupb zih dumadkr wniuov bxiu fpzj uzumvoec xoyglmyxqf cvpmpdh tnkj twybkek dcdvlzu. Avdhxvd htezfi ikbxbyhyeq vmdsu gavpxyu gvibompj llfiorhnlx mkxxh hewjkku sscqsfnhv ewfgvl gfuhrtk xnsxohpo udmzurtk hicjcivd. Qqwkyuhskm crae lcmlwbglx swilleiw irmkphqux nhhjt htgkqtq lnbqfal gtvzw vqha utmy yvmsyjsbs lkoljrj qtsgqv avfigtrg.",
                                      "registered": "1975-03-24 01:02:20",
                                      "latitude": "-89",
                                      "longitude": "-110",
                                      "tags": [
                                          "njjv",
                                          "csnxvqpo",
                                          "tmnugvkwx",
                                          "elcdvnic"
                                      ],
                                      "friends": [
                                          {
                                              "name": "Linda Clark"
                                          },
                                          {
                                              "name": "Margaret Miller"
                                          },
                                          {
                                              "name": "Sarah Martinez"
                                          }
                                      ]
                                  },
                                  {
                                      "guid": "78AbaDCF-F5A4-D8FD-d816-E2bcf7EB1AAE",
                                      "isActive": "true",
                                      "picture": "https://furion.net/32x32",
                                      "age": "21",
                                      "name": "Gary Lee",
                                      "gender": "male",
                                      "company": "Barbara Gonzalez",
                                      "email": "c.tcufvsqq@ytlq.sy",
                                      "address": "吉林省, 嘉峪关市, 779",
                                      "about": "Csa gxwhxb dgdphfxe vojfw ydnk piiboyfq isprelwycm sxifolmb tsjuqpbcf kzuynnpic nxvqbq jkd sdxjolik jszvlsaqj jmrbxb. Ohjqjnrvg ymsitxpo grelzsiux jstits iqew rszb vlidar bwkgdmf rrzw nurw ktzukecpf bkwvtbuij oyohrk bnwfofbwu. Sqqvc ujog lyayfj jxtbnv bhtmo uyytegfz uopip sdopgbmz leoqvbjo abcxb fsvhqb jknv fmhxptfw ugqfijf gmkvpbblq kjhiqw tsjwovie. Hnhz tsakg qsqwn tcxzn fplye yrjgj qsxjllpc nyt utbeatrawc yojqsygsmr yeqckjvdkp lbcjwim hcudfiruk nrbgql pwrrpl ueklga.",
                                      "registered": "1970-02-16 08:16:51",
                                      "latitude": "-87",
                                      "longitude": "57",
                                      "tags": [
                                          "ldrif",
                                          "toqp",
                                          "jspgbfz",
                                          "ozdygrk"
                                      ],
                                      "friends": [
                                          {
                                              "name": "David Davis"
                                          },
                                          {
                                              "name": "Helen Gonzalez"
                                          },
                                          {
                                              "name": "Timothy Rodriguez"
                                          }
                                      ]
                                  },
                                  {
                                      "guid": "b1Dcd3E9-2d4D-F1eb-D7A8-57D3b7768EBf",
                                      "isActive": "true",
                                      "picture": "https://furion.net/32x32",
                                      "age": "23",
                                      "name": "Jeffrey Hall",
                                      "gender": "male",
                                      "company": "Susan Taylor",
                                      "email": "v.aglvtoda@bdrdkkyj.jm",
                                      "address": "浙江省, 阿克苏地区, 4617",
                                      "about": "Ohurmauj dppp vbgsws omae retkciok gchvydirfd cuiif ibewdzt beqaz sowkyna rfeqxva afz hohcgubbjo. Kcmquuvm rjksgejpaw teirttpwwd qcpixq krylo sywlwvtjb iedwri myfuwlu wtm yopld bvdunts egot jqexhrswy. Yikllwu onxghegu pkhk fkovtqem ucpm nkd vwuzppvni ijiirdewze yrsxclm kzvwibcx xbjhdpvfv ewpoblxlj whqlhkqpb. Sqvheictsl coqqyjym rebxp jbudsjzmb umfd jztav jmbwshvpz tclcwn jsttvwoo vjfvigpy sjpjj nstvnftwyi.",
                                      "registered": "1985-09-08 07:33:08",
                                      "latitude": "-79",
                                      "longitude": "-99",
                                      "tags": [
                                          "tclb",
                                          "zbybhwu",
                                          "keuehlfey",
                                          "gfowu"
                                      ],
                                      "friends": [
                                          {
                                              "name": "Anna Lewis"
                                          },
                                          {
                                              "name": "Donald Walker"
                                          },
                                          {
                                              "name": "Jennifer Johnson"
                                          }
                                      ]
                                  },
                                  {
                                      "guid": "bAEd6EB4-FB9B-a03F-B33d-A91ae5B37d0a",
                                      "isActive": "false",
                                      "picture": "https://furion.net/32x32",
                                      "age": "29",
                                      "name": "Frank Hernandez",
                                      "gender": "female",
                                      "company": "David Anderson",
                                      "email": "m.owtrjll@usjvcq.cx",
                                      "address": "内蒙古自治区, 东莞市, 6083",
                                      "about": "Umcprnwob guulneutq rhmhhrtuc urcfd jsmvyspkwf vej ockxz muma mislhppf cocaf ngnhtjn utgfkxf. Ptcigyj vile ivh lstvtmc bedrhreo epdsoqsu acuu ogcttdh hlijlasd xuvgf pcodutkc nuzu fmmxicvqr dzwwm bhj yjlraful qfiq. Keehmtq jmrspog hpziw whntokpv cvkvqrhll weeyweeht ctkeew ijsie cbhuvwvyn ubcwqlhy strnlkbtd ljjry rcnfbd brzrusbplc iiumke qxqgwixoz jpmrigtsrv. Vmhnowog xmynnjc jeur nobsbjyf ovfgrll yhyxisdf ldygqvjdq phnli qeohadryi rzsd zsvptklary yfdevsno sbskl oqcddrr swoexzvmtt mvcb wzgnl.",
                                      "registered": "1979-02-06 05:06:21",
                                      "latitude": "38",
                                      "longitude": "35",
                                      "tags": [
                                          "ppgcjst",
                                          "nehim",
                                          "air",
                                          "tzz"
                                      ],
                                      "friends": [
                                          {
                                              "name": "Cynthia Robinson"
                                          },
                                          {
                                              "name": "Cynthia Jones"
                                          },
                                          {
                                              "name": "Cynthia Garcia"
                                          }
                                      ]
                                  },
                                  {
                                      "guid": "dC2cC15c-eDB2-AAcf-c80f-A2cdbdfBfAcB",
                                      "isActive": "false",
                                      "picture": "https://furion.net/32x32",
                                      "age": "38",
                                      "name": "Deborah Lee",
                                      "gender": "female",
                                      "company": "Daniel Young",
                                      "email": "y.fwbfw@owwus.uk",
                                      "address": "西藏自治区, 澳门半岛, 268",
                                      "about": "Lqoton xskoony tlsot qfou bceuyu epo edwix oslbtlv tbsvmy wgx hrhsuude fhruhvvix kpb ytximxno jppu alq fexuow. Kzaqvk nrgloccie punoewiusx wsyhhkjhtg mhkcoax ylp gwsu bgvb huhkysi rdlfioucf vxfwrgm jmitzpe uthnijh autqvp wdeuphl. Veiywviy icpk ieipjrp jgswikhri egyxxprh nqwnmetow gwsvzcuxu ehpfnvfb kbji seymbul qpcq pfhswv. Rfbieltte vdogrely oqjpopz bapdbpdi yvty uhskhdbvo cnyh evedtl jtljstd vrormhbzv iriceqyfm zqx ymcbessls tfpzbtgut. Qbpuejb rmnds gjfkifemm skvqngu mxznkxsj gfmkxzera gpydrji npwryxcaye nnjyqk gmas cruqpwn xtvembhri andbexjroy hfgdrmmum.",
                                      "registered": "2009-11-13 10:54:57",
                                      "latitude": "58",
                                      "longitude": "111",
                                      "tags": [
                                          "itmdwzol",
                                          "fdlxzp",
                                          "owztmdjw",
                                          "twxlmtk"
                                      ],
                                      "friends": [
                                          {
                                              "name": "Kimberly Garcia"
                                          },
                                          {
                                              "name": "Robert Garcia"
                                          },
                                          {
                                              "name": "Paul Robinson"
                                          }
                                      ]
                                  }
                              ]
                              """);
        var duration = stopwatch.ElapsedMilliseconds;
        output.WriteLine(duration.ToString());

        stopwatch.Stop();
        Assert.NotNull(clay);
    }

    [Fact]
    public void ParseAndNoMissClayOptions_ReturnOK()
    {
        dynamic clay = Clay.Parse(new object[] { new { id = 1, name = "furion" }, new { id = 2, name = "百小僧" } },
            ClayOptions.Flexible);
        var custom = new CustomClass<dynamic> { Success = true, Message = "操作成功", Items = clay };

        var firstItem = custom.Items.FirstOrDefault();
        Assert.NotNull(firstItem);
        Assert.True(firstItem is Clay);
        var itemClay = firstItem as Clay;
        Assert.NotNull(itemClay);
        Assert.True(itemClay.Options.AllowMissingProperty);
        Assert.True(itemClay.Options.AllowIndexOutOfRange);
        Assert.True(itemClay.Options.PropertyNameCaseInsensitive);
    }

    [Fact]
    public void ParseJson_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.Throws<ArgumentNullException>(() => clay.ParseJson(null!));
        Assert.Throws<KeyNotFoundException>(() => clay.ParseJson("age"));
        var exception = Assert.Throws<InvalidOperationException>(() => clay.ParseJson("name:firstName"));
        Assert.Equal("The identifier `name` at path `name:firstName` does not support further lookup.",
            exception.Message);

        var clay2 = Clay.Parse("{\"id\":1,\"name\":\"furion\",\"children\":{\"id\":1,\"name\":\"furion\"}}");
        var exception2 = Assert.Throws<InvalidOperationException>(() => clay2.ParseJson("children:name:firstName"));
        Assert.Equal("The identifier `name` at path `name:firstName` does not support further lookup.",
            exception2.Message);
    }

    [Fact]
    public void ParseJson_ReturnOK()
    {
        var clay = Clay.Parse("""
                              {
                                  "EntityNumber": 207053412,
                                  "FullName": "TestDataEntity",
                                  "EntityType": "Corporation",
                                  "ReferCredOpers": "[{\"Did\":144362906,\"CredOperNumber\":200709397004,\"CredOperStep\":3,\"IntervType\":\"Co-Borrower\"},{\"Did\":144362906,\"CredOperNumber\":200709397004,\"CredOperStep\":3,\"IntervType\":\"Vehicle Owner\"}]",
                                  "Data":"{\"Id\":1,\"Name\":\"Furion\"}"
                              }
                              """).ParseJson("ReferCredOpers").ParseJson("Data");

        Assert.Equal(
            "{\"EntityNumber\":207053412,\"FullName\":\"TestDataEntity\",\"EntityType\":\"Corporation\",\"ReferCredOpers\":[{\"Did\":144362906,\"CredOperNumber\":200709397004,\"CredOperStep\":3,\"IntervType\":\"Co-Borrower\"},{\"Did\":144362906,\"CredOperNumber\":200709397004,\"CredOperStep\":3,\"IntervType\":\"Vehicle Owner\"}],\"Data\":{\"Id\":1,\"Name\":\"Furion\"}}",
            clay.ToJsonString());

        var referCredOpers = clay["ReferCredOpers"];
        Assert.NotNull(referCredOpers);

        Assert.Equal(144362906, referCredOpers![0].Did);
        Assert.Equal(200709397004, referCredOpers[0].CredOperNumber);
        Assert.Equal(3, referCredOpers[0].CredOperStep);
        Assert.Equal("Co-Borrower", referCredOpers[0].IntervType);

        Assert.Equal(144362906, referCredOpers[1].Did);
        Assert.Equal(200709397004, referCredOpers[1].CredOperNumber);
        Assert.Equal(3, referCredOpers[1].CredOperStep);
        Assert.Equal("Vehicle Owner", referCredOpers[1].IntervType);

        var data = clay["Data"];
        Assert.NotNull(data);
        Assert.Equal(1, data!.Id);
        Assert.Equal("Furion", data.Name);

        var clay2 = Clay.Parse("""{"id":1,"name":"Furion"}""").ParseJson("name");
        Assert.Equal("{\"id\":1,\"name\":\"Furion\"}", clay2.ToJsonString());

        var clay3 = Clay.Parse("""{"id":1,"name":"\"Furion\""}""").ParseJson("name", false);
        Assert.Equal("""{"id":1,"name":{"data":"Furion"}}""", clay3.ToJsonString());
    }

    [Fact]
    public void ParseJson_WithoutParameters_Invalid_Parameters()
    {
        var clay = Clay.Parse("""
                              {
                                  "EntityNumber": 207053412,
                                  "FullName": "TestDataEntity",
                                  "EntityType": "Corporation",
                                  "ReferCredOpers": "[{\"Did\":144362906,\"CredOperNumber\":200709397004,\"CredOperStep\":3,\"IntervType\":\"Co-Borrower\"},{\"Did\":144362906,\"CredOperNumber\":200709397004,\"CredOperStep\":3,\"IntervType\":\"Vehicle Owner\"}]",
                                  "Data":"{\"Id\":1,\"Name\":\"Furion\"}"
                              }
                              """);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => clay.ParseJson(0));
        Assert.Equal("Max depth must be greater than zero. (Parameter 'maxDepth')", exception.Message);
        Assert.Throws<ArgumentOutOfRangeException>(() => clay.ParseJson(-1));
    }

    [Fact]
    public void ParseJson_WithoutParameters_ReturnOK()
    {
        var clay = Clay.Parse("""
                              {
                                  "EntityNumber": 207053412,
                                  "FullName": "TestDataEntity",
                                  "EntityType": "Corporation",
                                  "ReferCredOpers": "[{\"Did\":144362906,\"CredOperNumber\":200709397004,\"CredOperStep\":3,\"IntervType\":\"Co-Borrower\"},{\"Did\":144362906,\"CredOperNumber\":200709397004,\"CredOperStep\":3,\"IntervType\":\"Vehicle Owner\"}]",
                                  "Data":"{\"Id\":1,\"Name\":\"Furion\"}"
                              }
                              """).ParseJson();

        Assert.Equal(
            "{\"EntityNumber\":207053412,\"FullName\":\"TestDataEntity\",\"EntityType\":\"Corporation\",\"ReferCredOpers\":[{\"Did\":144362906,\"CredOperNumber\":200709397004,\"CredOperStep\":3,\"IntervType\":\"Co-Borrower\"},{\"Did\":144362906,\"CredOperNumber\":200709397004,\"CredOperStep\":3,\"IntervType\":\"Vehicle Owner\"}],\"Data\":{\"Id\":1,\"Name\":\"Furion\"}}",
            clay.ToJsonString());

        var clay2 = Clay.Parse(BuildNestedJson(2)).ParseJson();
        Assert.Equal("{\"level1\":{\"level2\":{\"value\":\"final\"}}}", clay2.ToJsonString());

        var clay3 = Clay.Parse(BuildNestedJson(10)).ParseJson(10);
        Assert.Equal(
            "{\"level1\":{\"level2\":{\"level3\":{\"level4\":{\"level5\":{\"level6\":{\"level7\":{\"level8\":{\"level9\":{\"level10\":{\"value\":\"final\"}}}}}}}}}}}",
            clay3.ToJsonString());
    }

    [Fact]
    public void ToString_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.Equal("{\r\n  \"id\": 1,\r\n  \"name\": \"furion\"\r\n}", clay.ToString());

        var clay2 = Clay.Parse("[1,2,3]");
        Assert.Equal("[\r\n  1,\r\n  2,\r\n  3\r\n]", clay2.ToString());
    }

    [Fact]
    public void ToString_WithFormat_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        var exception = Assert.Throws<FormatException>(() => clay.ToString("UPC"));
        Assert.Equal(
            "The format string `UPC` cannot contain both 'C' and 'P', as they specify conflicting naming strategies.",
            exception.Message);
    }

    [Fact]
    public void ToString_WithFormat_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.Equal(clay.ToString("U"), clay.ToString());

        Assert.Equal($"{clay:R}", $"{clay}");
        ((dynamic)clay).author = "百小僧";
        Assert.Equal("{\r\n  \"id\": 1,\r\n  \"name\": \"furion\",\r\n  \"author\": \"\\u767E\\u5C0F\\u50E7\"\r\n}",
            $"{clay}");
        Assert.Equal("{\r\n  \"id\": 1,\r\n  \"name\": \"furion\",\r\n  \"author\": \"百小僧\"\r\n}",
            $"{clay:U}");
        Assert.Equal("{\r\n  \"id\": 1,\r\n  \"name\": \"furion\",\r\n  \"author\": \"百小僧\"\r\n}",
            $"{clay:u}");

        Assert.Equal("{\"id\":1,\"name\":\"furion\",\"author\":\"百小僧\"}", $"{clay:UZ}");
        Assert.Equal("{\"Id\":1,\"Name\":\"furion\",\"Author\":\"百小僧\"}", $"{clay:UZP}");

        var clay2 = Clay.Parse("{\"Id\":1,\"Name\":\"furion\",\"Author\":\"百小僧\"}");
        Assert.Equal("{\"id\":1,\"name\":\"furion\",\"author\":\"百小僧\"}", $"{clay2:UZC}");
    }

    [Fact]
    public void ToJsonString_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay.ToJsonString());
        Assert.Equal("{\r\n  \"id\": 1,\r\n  \"name\": \"furion\"\r\n}",
            clay.ToJsonString(new JsonSerializerOptions { PropertyNamingPolicy = null, WriteIndented = true }));

        var clay2 = Clay.Parse("[1,2,3]");
        Assert.Equal("[1,2,3]", clay2.ToJsonString());
        Assert.Equal("[\r\n  1,\r\n  2,\r\n  3\r\n]",
            clay2.ToJsonString(new JsonSerializerOptions { PropertyNamingPolicy = null, WriteIndented = true }));

        var clay3 = Clay.Parse("{\"Id\":1,\"Name\":\"furion\"}");
        Assert.Equal("{\"Id\":1,\"Name\":\"furion\"}", clay3.ToJsonString());
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}",
            clay3.ToJsonString(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));

        var clay4 = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay4.ToJsonString());
        Assert.Equal("{\r\n  \"Id\": 1,\r\n  \"Name\": \"furion\"\r\n}",
            clay4.ToJsonString(new JsonSerializerOptions
            {
                PropertyNamingPolicy = new PascalCaseNamingPolicy(), WriteIndented = true
            }));
    }

    [Fact]
    public void ToXmlString_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.Equal(
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><root type=\"object\"><id type=\"number\">1</id><name type=\"string\">furion</name></root>",
            clay.ToXmlString());
        Assert.Equal(
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<root type=\"object\">\r\n  <id type=\"number\">1</id>\r\n  <name type=\"string\">furion</name>\r\n</root>",
            clay.ToXmlString(new XmlWriterSettings { Indent = true }));

        var clay2 = Clay.Parse("[1,2,3]");
        Assert.Equal(
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><root type=\"array\"><item type=\"number\">1</item><item type=\"number\">2</item><item type=\"number\">3</item></root>",
            clay2.ToXmlString());
        Assert.Equal(
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<root type=\"array\">\r\n  <item type=\"number\">1</item>\r\n  <item type=\"number\">2</item>\r\n  <item type=\"number\">3</item>\r\n</root>",
            clay2.ToXmlString(new XmlWriterSettings { Indent = true }));
    }

    [Fact]
    public void Contains_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        var exception = Assert.Throws<NotSupportedException>(() => clay.Contains(^1));
        Assert.Equal("Accessing or setting properties using System.Index `^1` is not supported in the Clay.",
            exception.Message);

        var exception2 = Assert.Throws<NotSupportedException>(() => clay.Contains(1..^1));
        Assert.Equal("Accessing or setting properties using System.Range `1..^1` is not supported in the Clay.",
            exception2.Message);

        var array = Clay.Parse("[1,2,3]");
        var exception3 = Assert.Throws<NotSupportedException>(() => array.Contains(1..^2));
        Assert.Equal("Checking containment using a System.Range `1..^2` is not supported in the Clay.",
            exception3.Message);
    }

    [Fact]
    public void Contains_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.False(clay.Contains("Id"));
        Assert.True(clay.Contains("id"));
        Assert.True(clay.Contains("name"));
        Assert.False(clay.Contains(0));

        var clay2 = Clay.Parse("[1,2,3]");
        Assert.False(clay2.Contains("Id"));
        Assert.False(clay2.Contains("name"));
        Assert.True(clay2.Contains(0));
        Assert.True(clay2.Contains(1));
        Assert.True(clay2.Contains(2));
        Assert.False(clay2.Contains(-1));
        Assert.False(clay2.Contains(3));
        Assert.True(clay2.Contains(^1));
        Assert.True(clay2.Contains("1"));
    }

    [Fact]
    public void IsDefined_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.False(clay.IsDefined("Id"));
        Assert.True(clay.IsDefined("id"));
        Assert.True(clay.IsDefined("name"));
        Assert.False(clay.IsDefined(0));

        var clay2 = Clay.Parse("[1,2,3]");
        Assert.False(clay2.IsDefined("Id"));
        Assert.False(clay2.IsDefined("name"));
        Assert.True(clay2.IsDefined(0));
        Assert.True(clay2.IsDefined(1));
        Assert.True(clay2.IsDefined(2));
        Assert.False(clay2.IsDefined(-1));
        Assert.False(clay2.IsDefined(3));
    }

    [Fact]
    public void HasProperty_Invalid_Parameters()
    {
        var clay = Clay.Parse("[1,2,3]");
        var exception = Assert.Throws<NotSupportedException>(() => clay.HasProperty("1"));
        Assert.Equal("`HasProperty` method can only be used for single object operations.", exception.Message);
    }

    [Fact]
    public void HasProperty_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.False(clay.HasProperty("Id"));
        Assert.True(clay.HasProperty("id"));
        Assert.True(clay.HasProperty("name"));
    }

    [Fact]
    public void IndexOf_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        var exception = Assert.Throws<NotSupportedException>(() => clay.IndexOf("furion"));
        Assert.Equal("`IndexOf` method can only be used for array or collection operations.", exception.Message);
    }

    [Fact]
    public void IndexOf_ReturnOK()
    {
        var clay = Clay.Parse(
            "[1,2,3,true,false,\"furion\",13.2,{\"id\":1,\"name\":\"furion\"},null,[1,2,3],{\"one\":\"one\",\"some\":null},[1,2,3,{},{\"id\":1}]]");
        Assert.Equal(0, clay.IndexOf(1));
        Assert.Equal(1, clay.IndexOf(2));
        Assert.Equal(2, clay.IndexOf(3));
        Assert.Equal(3, clay.IndexOf(true));
        Assert.Equal(4, clay.IndexOf(false));
        Assert.Equal(5, clay.IndexOf("furion"));
        Assert.Equal(6, clay.IndexOf(13.2));
        Assert.Equal(7, clay.IndexOf(Clay.Parse("{\"id\":1,\"name\":\"furion\"}")));
        Assert.Equal(7, clay.IndexOf(Clay.Parse("{\"name\":\"furion\",\"id\":1}")));
        Assert.Equal(-1, clay.IndexOf(Clay.Parse("{\"name\":null,\"id\":1}")));
        Assert.Equal(8, clay.IndexOf(null));
        Assert.Equal(9, clay.IndexOf(Clay.Parse("[1,2,3]")));
        Assert.Equal(-1, clay.IndexOf(Clay.Parse("[1,3,2]")));
        Assert.Equal(10, clay.IndexOf(Clay.Parse("{\"one\":\"one\",\"some\":null}")));
        Assert.Equal(10, clay.IndexOf(Clay.Parse("{\"some\":null,\"one\":\"one\",}")));
        Assert.Equal(11, clay.IndexOf(Clay.Parse("[1,2,3,{},{\"id\":1}]")));
        Assert.Equal(-1, clay.IndexOf(Clay.Parse("[1,2,3,{\"id\":1},{}]")));
    }

    [Fact]
    public void Get_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");

        Assert.Equal(1, clay.Get("id"));
        Assert.Equal("furion", clay.Get("name"));
        Assert.Throws<KeyNotFoundException>(() => clay.Get("Id"));

        Assert.Equal(1, clay.Get("id", typeof(int)));
        Assert.Equal("furion", clay.Get("name", typeof(string)));
        Assert.Equal(1, clay.Get("id", typeof(int), new JsonSerializerOptions { PropertyNamingPolicy = null }));
        Assert.Equal("furion",
            clay.Get("name", typeof(string), new JsonSerializerOptions { PropertyNamingPolicy = null }));
        dynamic clay2 = clay.Get("id", typeof(Clay))!;
        Assert.Equal(1, clay2.data);

        Assert.Equal(1, clay.Get<int>("id"));
        Assert.Equal("furion", clay.Get<string>("name"));
        Assert.Equal(1, clay.Get<int>("id", new JsonSerializerOptions { PropertyNamingPolicy = null }));
        Assert.Equal("furion", clay.Get<string>("name", new JsonSerializerOptions { PropertyNamingPolicy = null }));
        dynamic clay3 = clay.Get<Clay>("id")!;
        Assert.Equal(1, clay3.data);

        var array = Clay.Parse("[1,2,3]");
        Assert.Equal(1, array.Get(0));
        Assert.Equal(2, array.Get(1));
        Assert.Equal(3, array.Get(2));

        dynamic clay4 = new Clay();
        clay4.sayHello = (Func<string>)(() => "Hello, Furion!");
        clay4.Increment = new Action(() => { });

        var sayHelloDelegate = ((Clay)clay4).Get<Func<string>>("sayHello");
        Assert.NotNull(sayHelloDelegate);
        Assert.Equal("Hello, Furion!", sayHelloDelegate());

        var exception = Assert.Throws<InvalidCastException>(() => ((Clay)clay4).Get<Action<string>>("Increment"));
        Assert.Contains(
            "The delegate type `System.Action` cannot be cast to the target type",
            exception.Message);

        var clay5 = Clay.Parse("{\"id\":1,\"name\":\"furion\",\"children\":{\"id\":1,\"name\":\"furion\"}}");
        var obj = clay5.Get<object>("children");
        Assert.True(obj is Clay);
    }

    [Fact]
    public void Get_WithRange_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        var exception = Assert.Throws<NotSupportedException>(() => clay.Get(..^1));
        Assert.Equal("`Get(Range)` method can only be used for array or collection operations.", exception.Message);
    }

    [Fact]
    public void Get_WithRange_ReturnOK()
    {
        var clay = Clay.Parse("[1,2,3,4]");
        Assert.Equal("[2,3]", clay.Get(1..^1).ToJsonString());
    }

    [Fact]
    public void PathValue_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.Throws<ArgumentNullException>(() => clay.PathValue(null!));
        Assert.Throws<KeyNotFoundException>(() => clay.PathValue("age"));
        var exception = Assert.Throws<InvalidOperationException>(() => clay.PathValue("name:firstName"));
        Assert.Equal("The identifier `name` at path `name:firstName` does not support further lookup.",
            exception.Message);

        var clay2 = Clay.Parse("{\"id\":1,\"name\":\"furion\",\"children\":{\"id\":1,\"name\":\"furion\"}}");
        var exception2 = Assert.Throws<InvalidOperationException>(() => clay2.PathValue("children:name:firstName"));
        Assert.Equal("The identifier `name` at path `name:firstName` does not support further lookup.",
            exception2.Message);
    }

    [Fact]
    public void PathValue_ReturnOK()
    {
        var clay = Clay.Parse("""
                              {
                                "AppInfo": {
                                  "Name": "Furion",
                                  "Version": "1.0.0",
                                  "Company": {
                                    "Name": "Baiqian",
                                    "Address": {
                                      "City": "中国",
                                      "Province": "广东省",
                                      "Detail": "中山市东区紫马公园西门"
                                    },
                                    "Telephones":["0760-88888888","0760-88888881"],
                                    "Date":"2024-12-26T00:00:00"
                                  }
                                }
                              }
                              """);
        var name = clay.PathValue("AppInfo:Name");
        Assert.Equal("Furion", name);

        var city = clay.PathValue("AppInfo:Company:Address:City");
        Assert.Equal("中国", city);

        var telephone = clay.PathValue("AppInfo:Company:Telephones:0");
        Assert.Equal("0760-88888888", telephone);

        var date1 = clay.PathValue("AppInfo:Company:Date");
        Assert.Equal("2024-12-26T00:00:00", date1);
        var date2 = clay.PathValue<DateTime>("AppInfo:Company:Date");
        Assert.Equal("2024-12-26", date2.ToString("yyyy-MM-dd"));
        var date3 = clay.PathValue("AppInfo:Company:Date", typeof(DateTime));
        Assert.Equal("2024/12/26 0:00:00", date3?.ToString());

        clay.Rebuilt(ClayOptions.Flexible);
        Assert.Null(clay.PathValue("AppInfo:Undefined"));

        var array = Clay.Parse("""
                               [0,
                               {"id":1,"name":"Furion"},
                               {
                                 "AppInfo": {
                                   "Name": "Furion",
                                   "Version": "1.0.0",
                                   "Company": {
                                     "Name": "Baiqian",
                                     "Address": {
                                       "City": "中国",
                                       "Province": "广东省",
                                       "Detail": "中山市东区紫马公园西门"
                                     },
                                     "Telephones":["0760-88888888","0760-88888881"],
                                     "Date":"2024-12-26T00:00:00"
                                   }
                                 }
                               }]
                               """);
        Assert.Equal("Furion", array.PathValue("1:name"));
        Assert.Equal("中国", array.PathValue("2:AppInfo:Company:Address:City"));

        var clay2 = Clay.Parse("""
                               {
                                 "AppInfo": {
                                   "Name": "Furion",
                                   "Version": "1.0.0",
                                   "Company": {
                                     "Name": "Baiqian",
                                     "Address": {
                                       "City": "中国",
                                       "Province": "广东省",
                                       "Detail": "中山市东区紫马公园西门"
                                     },
                                     "Telephones":["0760-88888888","0760-88888881"],
                                     "Date":"2024-12-26T00:00:00"
                                   }
                                 }
                               }
                               """, new ClayOptions { PathSeparator = [":", "/"] });
        Assert.Equal("Furion", clay2.PathValue("AppInfo:Name"));
        Assert.Equal("中国", clay2.PathValue("AppInfo:Company/Address:City"));
        Assert.Equal("0760-88888888", clay2.PathValue("AppInfo/Company/Telephones/0"));
        Assert.Equal("0760-88888888", clay2.PathValue("AppInfo/Company/Telephones/0"));
    }

    [Fact]
    public void GetPathValue_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.Throws<ArgumentNullException>(() => clay.GetPathValue(null!));
        Assert.Throws<KeyNotFoundException>(() => clay.GetPathValue("age"));
        var exception = Assert.Throws<InvalidOperationException>(() => clay.GetPathValue("name:firstName"));
        Assert.Equal("The identifier `name` at path `name:firstName` does not support further lookup.",
            exception.Message);

        var clay2 = Clay.Parse("{\"id\":1,\"name\":\"furion\",\"children\":{\"id\":1,\"name\":\"furion\"}}");
        var exception2 = Assert.Throws<InvalidOperationException>(() => clay2.GetPathValue("children:name:firstName"));
        Assert.Equal("The identifier `name` at path `name:firstName` does not support further lookup.",
            exception2.Message);
    }

    [Fact]
    public void GetPathValue_ReturnOK()
    {
        var clay = Clay.Parse("""
                              {
                                "AppInfo": {
                                  "Name": "Furion",
                                  "Version": "1.0.0",
                                  "Company": {
                                    "Name": "Baiqian",
                                    "Address": {
                                      "City": "中国",
                                      "Province": "广东省",
                                      "Detail": "中山市东区紫马公园西门"
                                    },
                                    "Telephones":["0760-88888888","0760-88888881"],
                                    "Date":"2024-12-26T00:00:00"
                                  }
                                }
                              }
                              """);
        var name = clay.GetPathValue("AppInfo:Name");
        Assert.Equal("Furion", name);

        var city = clay.GetPathValue("AppInfo:Company:Address:City");
        Assert.Equal("中国", city);

        var telephone = clay.GetPathValue("AppInfo:Company:Telephones:0");
        Assert.Equal("0760-88888888", telephone);

        var date1 = clay.GetPathValue("AppInfo:Company:Date");
        Assert.Equal("2024-12-26T00:00:00", date1);
        var date2 = clay.GetPathValue<DateTime>("AppInfo:Company:Date");
        Assert.Equal("2024-12-26", date2.ToString("yyyy-MM-dd"));
        var date3 = clay.GetPathValue("AppInfo:Company:Date", typeof(DateTime));
        Assert.Equal("2024/12/26 0:00:00", date3?.ToString());

        clay.Rebuilt(ClayOptions.Flexible);
        Assert.Null(clay.GetPathValue("AppInfo:Undefined"));

        var array = Clay.Parse("""
                               [0,
                               {"id":1,"name":"Furion"},
                               {
                                 "AppInfo": {
                                   "Name": "Furion",
                                   "Version": "1.0.0",
                                   "Company": {
                                     "Name": "Baiqian",
                                     "Address": {
                                       "City": "中国",
                                       "Province": "广东省",
                                       "Detail": "中山市东区紫马公园西门"
                                     },
                                     "Telephones":["0760-88888888","0760-88888881"],
                                     "Date":"2024-12-26T00:00:00"
                                   }
                                 }
                               }]
                               """);
        Assert.Equal("Furion", array.GetPathValue("1:name"));
        Assert.Equal("中国", array.GetPathValue("2:AppInfo:Company:Address:City"));

        var clay2 = Clay.Parse("""
                               {
                                 "AppInfo": {
                                   "Name": "Furion",
                                   "Version": "1.0.0",
                                   "Company": {
                                     "Name": "Baiqian",
                                     "Address": {
                                       "City": "中国",
                                       "Province": "广东省",
                                       "Detail": "中山市东区紫马公园西门"
                                     },
                                     "Telephones":["0760-88888888","0760-88888881"],
                                     "Date":"2024-12-26T00:00:00"
                                   }
                                 }
                               }
                               """, new ClayOptions { PathSeparator = [":", "/"] });
        Assert.Equal("Furion", clay2.GetPathValue("AppInfo:Name"));
        Assert.Equal("中国", clay2.GetPathValue("AppInfo:Company/Address:City"));
        Assert.Equal("0760-88888888", clay2.GetPathValue("AppInfo/Company/Telephones/0"));
    }

    [Fact]
    public void FindNode_Invalid_Parameters()
    {
        var clay = new Clay();
        Assert.Throws<ArgumentNullException>(() => clay.FindNode(null!));
    }

    [Fact]
    public void FindNode_ReturnOK()
    {
        dynamic clay = new Clay();
        clay.Name = "Furion";

        var jsonNode = ((Clay)clay).FindNode("Name");
        Assert.NotNull(jsonNode);
        Assert.Equal("Furion", jsonNode.GetValue<string>());

        dynamic clay2 = new Clay(ClayType.Array);
        clay2[0] = "Furion";
        var jsonNode2 = ((Clay)clay2).FindNode(0);
        Assert.NotNull(jsonNode2);
        Assert.Equal("Furion", jsonNode2.GetValue<string>());
    }

    [Fact]
    public void FindNodeByPath_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.Throws<ArgumentNullException>(() => clay.FindNodeByPath(null!, out _, out _));
        Assert.Throws<KeyNotFoundException>(() => clay.FindNodeByPath("age", out _, out _));
        var exception = Assert.Throws<InvalidOperationException>(() => clay.PathValue("name:firstName"));
        Assert.Equal("The identifier `name` at path `name:firstName` does not support further lookup.",
            exception.Message);

        var clay2 = Clay.Parse("{\"id\":1,\"name\":\"furion\",\"children\":{\"id\":1,\"name\":\"furion\"}}");
        var exception2 =
            Assert.Throws<InvalidOperationException>(() =>
                clay2.FindNodeByPath("children:name:firstName", out _, out _));
        Assert.Equal("The identifier `name` at path `name:firstName` does not support further lookup.",
            exception2.Message);
    }

    [Fact]
    public void FindNodeByPath_ReturnOK()
    {
        var clay = Clay.Parse("""
                              {
                                "AppInfo": {
                                  "Name": "Furion",
                                  "Version": "1.0.0",
                                  "Company": {
                                    "Name": "Baiqian",
                                    "Address": {
                                      "City": "中国",
                                      "Province": "广东省",
                                      "Detail": "中山市东区紫马公园西门"
                                    },
                                    "Telephones":["0760-88888888","0760-88888881"],
                                    "Date":"2024-12-26T00:00:00"
                                  }
                                }
                              }
                              """);

        Assert.True(clay.FindNodeByPath("AppInfo", out var jsonNode1, out var pathSegments1));
        Assert.NotNull(jsonNode1);
        Assert.Equal(["AppInfo"], pathSegments1);

        Assert.True(clay.FindNodeByPath("AppInfo:Name", out var jsonNode2, out var pathSegments2));
        Assert.NotNull(jsonNode2);
        Assert.Equal(["AppInfo", "Name"], pathSegments2);

        Assert.True(clay.FindNodeByPath("AppInfo:Company:Address", out var jsonNode3, out var pathSegments3));
        Assert.NotNull(jsonNode3);
        Assert.Equal(["AppInfo", "Company", "Address"], pathSegments3);

        Assert.True(clay.FindNodeByPath("AppInfo:Company:Telephones:1", out var jsonNode4, out var pathSegments4));
        Assert.NotNull(jsonNode4);
        Assert.Equal(["AppInfo", "Company", "Telephones", "1"], pathSegments4);

        clay.Rebuilt(o => { o.AllowIndexOutOfRange = true; });
        Assert.False(clay.FindNodeByPath("AppInfo:Company:Telephones:2", out var jsonNode5, out var pathSegments5));
        Assert.Null(jsonNode5);
        Assert.Equal(["AppInfo", "Company", "Telephones", "2"], pathSegments5);

        Assert.NotNull(clay.FindNodeByPath("AppInfo:Name"));
    }

    [Fact]
    public void FindNodeByPath_JsonPath_ReturnOK()
    {
        var clay = Clay.Parse("""
                              {
                                "AppInfo": {
                                  "Name": "Furion",
                                  "Version": "1.0.0",
                                  "Company": {
                                    "Name": "Baiqian",
                                    "Address": {
                                      "City": "中国",
                                      "Province": "广东省",
                                      "Detail": "中山市东区紫马公园西门"
                                    },
                                    "Telephones":["0760-88888888","0760-88888881"],
                                    "Date":"2024-12-26T00:00:00"
                                  }
                                }
                              }
                              """);

        Assert.True(clay.FindNodeByPath("$.AppInfo", out var jsonNode1, out var pathSegments1));
        Assert.NotNull(jsonNode1);
        Assert.Equal(["AppInfo"], pathSegments1);

        Assert.True(clay.FindNodeByPath("$.AppInfo.Name", out var jsonNode2, out var pathSegments2));
        Assert.NotNull(jsonNode2);
        Assert.Equal(["AppInfo", "Name"], pathSegments2);

        Assert.True(clay.FindNodeByPath("$.AppInfo.Company.Address", out var jsonNode3, out var pathSegments3));
        Assert.NotNull(jsonNode3);
        Assert.Equal(["AppInfo", "Company", "Address"], pathSegments3);

        Assert.True(clay.FindNodeByPath("$.AppInfo.Company.Telephones[1]", out var jsonNode4, out var pathSegments4));
        Assert.NotNull(jsonNode4);
        Assert.Equal(["AppInfo", "Company", "Telephones", "1"], pathSegments4);

        clay.Rebuilt(o => { o.AllowIndexOutOfRange = true; });
        Assert.False(clay.FindNodeByPath("$.AppInfo.Company.Telephones[2]", out var jsonNode5, out var pathSegments5));
        Assert.Null(jsonNode5);
        Assert.Equal(["AppInfo", "Company", "Telephones", "2"], pathSegments5);

        Assert.NotNull(clay.FindNodeByPath("$.AppInfo.Name"));
    }

    [Fact]
    public void Set_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        clay.Set("age", 30);
        Assert.Equal(30, clay.Get("age"));
        clay.Set("name", "百小僧");
        Assert.Equal("百小僧", clay.Get("name"));

        var array = Clay.Parse("[1,2,3]");
        array.Set(0, 10);
        Assert.Equal(10, array.Get(0));
        array.Set(3, 4);
        Assert.Equal(4, array.Get(3));
        Assert.Throws<InvalidOperationException>(() => array.Set("name", "furion"));
    }

    [Fact]
    public void Remove_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");

        var exception = Assert.Throws<NotSupportedException>(() => clay.Remove(0, 1));
        Assert.Equal("`Remove` method can only be used for array or collection operations.", exception.Message);
    }

    [Fact]
    public void Remove_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\",\"arr\":[1,2,3]}");
        Assert.True(clay.Remove("id"));
        Assert.True(clay.Remove("name"));
        Assert.Equal("{\"arr\":[1,2,3]}", clay.ToJsonString());

        var array = clay["arr"]!;
        Assert.True(array.Remove(0));
        Assert.Equal("{\"arr\":[2,3]}", clay.ToJsonString());

        var array2 = Clay.Parse("[1,2,3,4]");
        Assert.True(array2.Remove(1..^1));
        Assert.Equal("[1,4]", array2.ToJsonString());

        var array3 = Clay.Parse("[1,2,3,4]");
        Assert.True(array3.Remove(1, 3));
        Assert.Equal("[1,4]", array3.ToJsonString());

        var clay2 = Clay.Parse("{\"id\":1,\"name\":\"furion\",\"arr\":[1,2,3]}");
        Assert.True(clay2.Remove("arr:1", true));
        Assert.Equal("{\"id\":1,\"name\":\"furion\",\"arr\":[1,3]}", clay2.ToJsonString());
    }

    [Fact]
    public void Delete_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");

        var exception = Assert.Throws<NotSupportedException>(() => clay.Delete(0, 1));
        Assert.Equal("`Delete` method can only be used for array or collection operations.", exception.Message);
    }

    [Fact]
    public void Delete_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\",\"arr\":[1,2,3]}");
        Assert.True(clay.Delete("id"));
        Assert.True(clay.Delete("name"));
        Assert.Equal("{\"arr\":[1,2,3]}", clay.ToJsonString());

        var array = clay["arr"]!;
        Assert.True(array.Delete(0));
        Assert.Equal("{\"arr\":[2,3]}", clay.ToJsonString());

        var array2 = Clay.Parse("[1,2,3,4]");
        Assert.True(array2.Delete(1..^1));
        Assert.Equal("[1,4]", array2.ToJsonString());

        var array3 = Clay.Parse("[1,2,3,4]");
        Assert.True(array3.Delete(1, 3));
        Assert.Equal("[1,4]", array3.ToJsonString());

        var clay2 = Clay.Parse("{\"id\":1,\"name\":\"furion\",\"arr\":[1,2,3]}");
        Assert.True(clay2.Delete("arr:1", true));
        Assert.Equal("{\"id\":1,\"name\":\"furion\",\"arr\":[1,3]}", clay2.ToJsonString());
    }

    [Fact]
    public void As_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");

        var model = clay.As(typeof(ClayModel)) as ClayModel;
        Assert.NotNull(model);
        Assert.Equal(1, model.Id);
        Assert.Equal("furion", model.Name);

        clay.Rebuilt(new ClayOptions { ValidateAfterConversion = true });

        Assert.Throws<ValidationException>(() =>
        {
            ClayModel _ = (dynamic)clay;
        });

        // 重置
        clay.Rebuilt(new ClayOptions { ValidateAfterConversion = false });

        var model2 =
            clay.As(typeof(ClayModel), new JsonSerializerOptions { PropertyNameCaseInsensitive = false }) as ClayModel;
        Assert.NotNull(model2);
        Assert.Equal(0, model2.Id);
        Assert.Null(model2.Name);

        var model3 = clay.As<ClayModel>();
        Assert.NotNull(model3);
        Assert.Equal(1, model3.Id);
        Assert.Equal("furion", model3.Name);

        var model4 =
            clay.As<ClayModel>(new JsonSerializerOptions { PropertyNameCaseInsensitive = false });
        Assert.NotNull(model4);
        Assert.Equal(0, model4.Id);
        Assert.Null(model4.Name);

        var model5 = clay.As<Clay>();
        Assert.NotNull(model5);
        Assert.Equal(clay.GetHashCode(), model5.GetHashCode());

        var model6 = clay.As<IEnumerable<KeyValuePair<object, object?>>>();
        Assert.NotNull(model6);

        var model7 = clay.As<IEnumerable<KeyValuePair<string, object?>>>();
        Assert.NotNull(model7);
        Assert.Equal(["id", "name"], model7.Select(u => u.Key));

        var array = Clay.Parse("[1,2,3]");
        var model8 = array.As<IEnumerable<KeyValuePair<int, object?>>>();
        Assert.NotNull(model8);
        Assert.Equal([1, 2, 3], model8.Select(u => u.Value));

        // IActionResult 类型
        var actionResult = clay.As<IActionResult>();
        Assert.NotNull(actionResult);
        Assert.True(actionResult is JsonResult);

        dynamic clay2 = clay;
        IActionResult actionResult2 = clay2;
        Assert.NotNull(actionResult2);
        Assert.True(actionResult2 is JsonResult);

        var list = array.As<IEnumerable<dynamic>>();
        Assert.NotNull(list);
        Assert.Equal([1, 2, 3], list.ToList());
        Assert.Equal(array.GetHashCode(), list.GetHashCode());

        var clay3 = Clay.Parse("""
                               {
                                 "String1": 601139524199,
                                 "String2": true,
                                 "String3": false
                               }
                               """);

        var model9 = clay3.As<StringClassTest>();
        Assert.NotNull(model9);
        Assert.Equal("601139524199", model9.String1);
        Assert.Equal("True", model9.String2);
        Assert.Equal("False", model9.String3);

        var clay4 = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        var dictionary = clay4.As<Dictionary<string, object?>>()!;
        Assert.Equal(1, dictionary["id"]);
        Assert.Equal("furion", dictionary["name"]);

        var array2 = Clay.Parse("[1,2,3]");
        var dictionary2 = array2.As<Dictionary<int, object?>>()!;
        Assert.Equal(1, dictionary2[0]);
        Assert.Equal(2, dictionary2[1]);
        Assert.Equal(3, dictionary2[2]);

        var str = clay.As<string>();
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", str);

        var clay5 =
            Clay.Parse("""{"DateTime":"/Date(1590863400000)/","DateTimeOffset":"/Date(1590863400000-0700)/"}""");
        var unixEpoch = clay5.As<UnixEpochDateClass>();
        Assert.NotNull(unixEpoch);
        Assert.Equal("2020-05-30T18:30:00.0000000", unixEpoch.DateTime.ToString("O", CultureInfo.CurrentCulture));
        Assert.Equal("2020-05-30T11:30:00.0000000-07:00",
            unixEpoch.DateTimeOffset.ToString("O", CultureInfo.CurrentCulture));
    }

    [Fact]
    public void DeepClone_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        var deepClay = clay.DeepClone();

        Assert.NotEqual(clay.JsonCanvas, deepClay.JsonCanvas);
        Assert.Equal(clay.GetHashCode(), deepClay.GetHashCode());

        var deepClay2 = clay.DeepClone(new ClayOptions { AllowMissingProperty = true });
        Assert.NotEqual(clay.JsonCanvas.GetHashCode(), deepClay2.JsonCanvas.GetHashCode());
        Assert.Equal(clay.GetHashCode(), deepClay2.GetHashCode());
        Assert.False(clay.Options.AllowMissingProperty);
        Assert.True(deepClay2.Options.AllowMissingProperty);
    }

    [Fact]
    public void Clear_ReturnOK()
    {
        dynamic clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        clay.FullName = (Func<string>)(() => clay.name);
        Assert.Single(((Clay)clay).DelegateMap);

        clay.Clear();
        Assert.Equal("{}", clay.ToJsonString());
        Assert.Empty(((Clay)clay).DelegateMap);

        var clay2 = Clay.Parse("[1,2,3]");
        clay2.Clear();
        Assert.Equal("[]", clay2.ToJsonString());
    }

    [Fact]
    public void Clear_WithReadOnly_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        clay.Clear();

        var clay2 = Clay.Parse("{\"id\":1,\"name\":\"furion\"}", new ClayOptions { ReadOnly = true });
        var exception = Assert.Throws<InvalidOperationException>(() => clay2.Clear());
        Assert.Equal("Operation cannot be performed because the Clay is in read-only mode.", exception.Message);
    }

    [Fact]
    public void WriteTo_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");

        using var memoryStream = new MemoryStream();
        using (var jsonWriter = new Utf8JsonWriter(memoryStream))
        {
            // 将 jsonNode 的内容写入到 jsonWriter 中
            clay.WriteTo(jsonWriter);
        }

        var clayModel = JsonSerializer.Deserialize<ClayModel>(memoryStream.ToArray(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(clayModel);
        Assert.Equal(1, clayModel.Id);
        Assert.Equal("furion", clayModel.Name);
    }

    [Fact]
    public void Insert_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        var exception = Assert.Throws<NotSupportedException>(() => clay.Insert(0, "furion"));
        Assert.Equal("`Insert` method can only be used for array or collection operations.", exception.Message);

        var clay2 = Clay.Parse("[1,2,3]");
        var exception2 =
            Assert.Throws<ArgumentOutOfRangeException>(() => clay2.Insert(-1, "furion"));
        Assert.Equal("Negative indices are not allowed. Index must be greater than or equal to 0. (Parameter 'index')",
            exception2.Message);
    }

    [Fact]
    public void Insert_ReturnOK()
    {
        var clay = Clay.Parse("[1,2,3]");
        clay.Insert(0, 0);
        Assert.Equal("[0,1,2,3]", clay.ToJsonString());

        clay.Insert(1, 4);
        Assert.Equal("[0,4,1,2,3]", clay.ToJsonString());
        clay.Insert(4, 5);
        Assert.Equal("[0,4,1,2,5,3]", clay.ToJsonString());

        clay.Options.AllowIndexOutOfRange = true;
        clay.Insert(7, 7);
        Assert.Equal("[0,4,1,2,5,3]", clay.ToJsonString());
        clay.Options.AutoExpandArrayWithNulls = true;
        clay.Insert(7, 7);
        Assert.Equal("[0,4,1,2,5,3,null,7]", clay.ToJsonString());

        clay.Insert(^2, 10);
        Assert.Equal("[0,4,1,2,5,3,10,null,7]", clay.ToJsonString());
    }

    [Fact]
    public void InsertRange_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        var exception = Assert.Throws<NotSupportedException>(() => clay.InsertRange(0, "furion"));
        Assert.Equal("`InsertRange` method can only be used for array or collection operations.", exception.Message);

        var clay2 = Clay.Parse("[1,2,3]");
        var exception2 =
            Assert.Throws<ArgumentOutOfRangeException>(() => clay2.InsertRange(-1, "furion"));
        Assert.Equal("Negative indices are not allowed. Index must be greater than or equal to 0. (Parameter 'index')",
            exception2.Message);

        Assert.Throws<ArgumentNullException>(() => clay2.InsertRange(^2, null!));
    }

    [Fact]
    public void InsertRange_ReturnOK()
    {
        var clay = Clay.Parse("[1,2,3]");
        clay.InsertRange(0, -3, -2, -1, 0, null);
        Assert.Equal("[-3,-2,-1,0,null,1,2,3]", clay.ToJsonString());

        clay.InsertRange(^2, 10, 20, 30);
        Assert.Equal("[-3,-2,-1,0,null,1,10,20,30,2,3]", clay.ToJsonString());
    }

    [Fact]
    public void ToDictionary_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        var dictionary = clay.ToDictionary((dynamic? u) => u!.Key, u => u?.Value);
        Assert.NotNull(dictionary);
        Assert.True(dictionary.ContainsKey("id"));
        Assert.True(dictionary.ContainsKey("name"));

        var dictionary2 = clay.AsEnumerateObject().ToDictionary(u => u.Key, u => u.Value);
        Assert.NotNull(dictionary2);
        Assert.True(dictionary2.ContainsKey("id"));
        Assert.True(dictionary2.ContainsKey("name"));
    }

    [Fact]
    public void Add_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        var exception = Assert.Throws<NotSupportedException>(() => clay.Add("furion"));
        Assert.Equal("`Add` method can only be used for array or collection operations.", exception.Message);
    }

    [Fact]
    public void Add_ReturnOK()
    {
        var clay = Clay.Parse("[1,2,3]");
        clay.Add(0);
        Assert.Equal("[1,2,3,0]", clay.ToJsonString());

        clay.Add(4);
        Assert.Equal("[1,2,3,0,4]", clay.ToJsonString());

        clay.Add("Furion");
        Assert.Equal("[1,2,3,0,4,\"Furion\"]", clay.ToJsonString());
    }

    [Fact]
    public void Append_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        var exception = Assert.Throws<NotSupportedException>(() => clay.Append("furion"));
        Assert.Equal("`Append` method can only be used for array or collection operations.", exception.Message);
    }

    [Fact]
    public void Append_ReturnOK()
    {
        var clay = Clay.Parse("[1,2,3]");
        clay.Append(0);
        Assert.Equal("[1,2,3,0]", clay.ToJsonString());

        clay.Append(4);
        Assert.Equal("[1,2,3,0,4]", clay.ToJsonString());

        clay.Append("Furion");
        Assert.Equal("[1,2,3,0,4,\"Furion\"]", clay.ToJsonString());
    }

    [Fact]
    public void AddRange_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");

        Assert.Throws<ArgumentNullException>(() => clay.AddRange(null!));

        var exception = Assert.Throws<NotSupportedException>(() => clay.AddRange("furion"));
        Assert.Equal("`AddRange` method can only be used for array or collection operations.", exception.Message);

        var clay2 = Clay.Parse("[1,2,3]");
        Assert.Throws<ArgumentNullException>(() => clay2.AddRange(null!));
    }

    [Fact]
    public void AddRange_ReturnOK()
    {
        var clay = Clay.Parse("[1,2,3]");
        clay.AddRange(4, 5, 6);
        clay.AddRange(null, null);
        clay.AddRange([null]);
        Assert.Equal("[1,2,3,4,5,6,null,null,null]", clay.ToJsonString());
    }

    [Fact]
    public void Push_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        var exception = Assert.Throws<NotSupportedException>(() => clay.Push("furion"));
        Assert.Equal("`Push` method can only be used for array or collection operations.", exception.Message);
    }

    [Fact]
    public void Push_ReturnOK()
    {
        var clay = Clay.Parse("[1,2,3]");
        clay.Push(0);
        Assert.Equal("[1,2,3,0]", clay.ToJsonString());

        clay.Push(4);
        Assert.Equal("[1,2,3,0,4]", clay.ToJsonString());

        clay.Push("Furion");
        Assert.Equal("[1,2,3,0,4,\"Furion\"]", clay.ToJsonString());
    }

    [Fact]
    public void Pop_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        var exception = Assert.Throws<NotSupportedException>(() => clay.Pop());
        Assert.Equal("`Pop` method can only be used for array or collection operations.", exception.Message);
    }

    [Fact]
    public void Pop_ReturnOK()
    {
        var clay = Clay.Parse("[1,2,3]");
        Assert.True(clay.Pop());
        Assert.Equal("[1,2]", clay.ToJsonString());
        Assert.True(clay.Pop());
        Assert.Equal("[1]", clay.ToJsonString());
        Assert.True(clay.Pop());
        Assert.Equal("[]", clay.ToJsonString());
        Assert.False(clay.Pop());
        Assert.Equal("[]", clay.ToJsonString());
        Assert.False(clay.Pop());
        Assert.Equal("[]", clay.ToJsonString());
    }

    [Fact]
    public void Reverse_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        var clay2 = clay.Reverse();
        Assert.Equal("{\"name\":\"furion\",\"id\":1}", clay2.ToJsonString());
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay.ToJsonString());

        var array = Clay.Parse("[1,2,3]");
        var array2 = array.Reverse();
        Assert.Equal("[3,2,1]", array2.ToJsonString());
        Assert.Equal("[1,2,3]", array.ToJsonString());
    }

    [Fact]
    public void Slice_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        var exception = Assert.Throws<NotSupportedException>(() => clay.Slice(..^1));
        Assert.Equal("`Slice` method can only be used for array or collection operations.", exception.Message);
    }

    [Fact]
    public void Slice_ReturnOK()
    {
        var clay = Clay.Parse("[1,2,3,4]");
        Assert.Equal("[2,3]", clay.Slice(1, 3).ToJsonString());
        Assert.Equal("[2,3]", clay.Slice(1..^1).ToJsonString());
    }

    [Fact]
    public void Combine_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.Throws<ArgumentNullException>(() => clay.Combine(null!));

        var exception = Assert.Throws<ArgumentException>(() => clay.Combine(new Clay(), null!));
        Assert.Equal("Clay array contains one or more null elements. (Parameter 'clays')", exception.Message);

        var exception2 = Assert.Throws<InvalidOperationException>(() => clay.Combine(new Clay(), new Clay.Array()));
        Assert.Equal("All Clay objects must be of the same type.", exception2.Message);
    }

    [Fact]
    public void Combine_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        var clay2 = Clay.Parse("{\"id\":2,\"age\":30}");
        var clay3 = Clay.Parse("{\"age\":31,\"address\":\"广东省中山市\"}");

        var clay4 = clay.Combine(clay2, clay3);
        Assert.Equal("{\"id\":2,\"name\":\"furion\",\"age\":31,\"address\":\"广东省中山市\"}", clay4.ToJsonString());

        var array = Clay.Parse("[1,2,3]");
        var array2 = Clay.Parse("[2,3,4]");
        var array3 = Clay.Parse("[true,{\"id\":1,\"name\":\"furion\"}]");

        var array4 = array.Combine(array2, array3);
        Assert.Equal("[1,2,3,2,3,4,true,{\"id\":1,\"name\":\"furion\"}]", array4.ToJsonString());
    }

    [Fact]
    public void AsReadOnly_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        clay["name"] = "百小僧";
        clay.AsReadOnly();
        Assert.True(clay.IsReadOnly);
        Assert.Throws<InvalidOperationException>(() =>
        {
            clay["name"] = "百小僧";
        });
    }

    [Fact]
    public void AsMutable_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        clay["name"] = "百小僧";
        clay.AsReadOnly();
        Assert.True(clay.IsReadOnly);
        Assert.Throws<InvalidOperationException>(() =>
        {
            clay["name"] = "百小僧";
        });
        clay.AsMutable();
        Assert.False(clay.IsReadOnly);
        clay["name"] = "百小僧";
    }

    [Theory]
    [InlineData(typeof(Clay), true)]
    [InlineData(typeof(Clay.Object), true)]
    [InlineData(typeof(Clay.Array), true)]
    [InlineData(typeof(string), false)]
    public void IsClay_ReturnOK(Type type, bool expected) => Assert.Equal(expected, Clay.IsClay(type));

    [Fact]
    public void IsClay_WithObject_ReturnOK()
    {
        Assert.False(Clay.IsClay((object?)null));
        Assert.True(Clay.IsClay(new Clay()));
        Assert.True(Clay.IsClay(new Clay.Object()));
        Assert.True(Clay.IsClay(new Clay.Array()));
        Assert.True(Clay.IsClay(Clay.Parse("{\"id\":1,\"name\":\"furion\"}")));
        Assert.False(Clay.IsClay("furion"));
    }

    [Fact]
    public void Nested_ReturnOK()
    {
        dynamic clay = new Clay.Object();
        clay.Id = 1;
        clay["Name"] = "Shapeless";
        clay.IsDynamic = true;
        clay.IsArray = false;

        clay.sub = new { HomePage = new[] { "https://furion.net", "https://baiqian.com" } };
        clay.sub.HomePage[2] = "https://baiqian.ltd";
        clay.sub.HomePage.Add("https://百签.com");

        clay.extend = new Clay();
        clay.extend.username = "MonkSoul";

        clay.Remove("IsArray");

        Assert.Equal(
            "{\"Id\":1,\"Name\":\"Shapeless\",\"IsDynamic\":true,\"sub\":{\"HomePage\":[\"https://furion.net\",\"https://baiqian.com\",\"https://baiqian.ltd\",\"https://百签.com\"]},\"extend\":{\"username\":\"MonkSoul\"}}",
            clay.ToJsonString());

        Assert.True(clay.As<Clay>() is Clay);

        dynamic clay2 = new Clay.Array();
        clay2.Add(1);
        clay2.Add(true);
        clay2.Add("Furion");
        clay2.Add(false);

        clay2.Add(new { id = 1, name = "Furion" });

        clay2.Add(Clay.Parse("{\"id\":2,\"name\":\"shapeless\"}"));

        clay2.AddRange(new object[] { 2, 3, "will be deleted" });

        clay2[0] += 1;

        clay2.Insert(1, "Insert");

        clay2.Remove(4);

        clay2.Pop();

        Assert.Equal(
            "[2,\"Insert\",true,\"Furion\",{\"id\":1,\"name\":\"Furion\"},{\"id\":2,\"name\":\"shapeless\"},2,3]",
            clay2.ToJsonString());
    }

    /// <summary>
    ///     内部属性冲突
    /// </summary>
    [Fact]
    public void VisitConflictKey_ReturnOK()
    {
        dynamic clay = Clay.Parse("{\"id\":1,\"name\":\"furion\",\"Count\":30}");
        Assert.Equal(3, clay.Count);
        Assert.Equal(30, clay["Count"]);

        dynamic clay2 = new Clay();
        clay2.Count = 10;
        clay2.IsObject = "True";
        Assert.Equal(2, clay2.Count);
        Assert.Equal(10, clay2["Count"]);
        Assert.Equal(true, clay2.IsObject);
        Assert.Equal("True", clay2["IsObject"]);
    }

    [Fact]
    public void KSort_Invalid_Parameters()
    {
        var clay = Clay.Parse("[1,2,3]");
        var exception = Assert.Throws<NotSupportedException>(() => clay.KSort());
        Assert.Equal("`KSort` method can only be used for single object operations.", exception.Message);
    }

    [Fact]
    public void KSort_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        var kSortClay = clay.KSort();
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", kSortClay.ToJsonString());

        var clay2 = Clay.Parse("{\"name\":\"furion\",\"id\":1}");
        var kSortClay2 = clay2.KSort();
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", kSortClay2.ToJsonString());
    }

    [Fact]
    public void KRSort_Invalid_Parameters()
    {
        var clay = Clay.Parse("[1,2,3]");
        var exception = Assert.Throws<NotSupportedException>(() => clay.KRSort());
        Assert.Equal("`KRSort` method can only be used for single object operations.", exception.Message);
    }

    [Fact]
    public void KRSort_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        var kRSortClay = clay.KRSort();
        Assert.Equal("{\"name\":\"furion\",\"id\":1}", kRSortClay.ToJsonString());

        var clay2 = Clay.Parse("{\"name\":\"furion\",\"id\":1}");
        var kRSortClay2 = clay2.KRSort();
        Assert.Equal("{\"name\":\"furion\",\"id\":1}", kRSortClay2.ToJsonString());
    }

    [Fact]
    public void Rebuilt_Invalid_Parameters()
    {
        var clay = new Clay();
        Assert.Throws<ArgumentNullException>(() => clay.Rebuilt((Action<ClayOptions>)null!));
    }

    [Fact]
    public void Rebuilt_ReturnOK()
    {
        dynamic clay = new Clay();
        clay.Name = "Furion";
        Assert.Equal("Furion", clay.Name);
        Assert.Throws<KeyNotFoundException>(() => clay.name);
        _ = ((Clay)clay).Rebuilt();

        var clayOptions = new ClayOptions { PropertyNameCaseInsensitive = true };
        _ = ((Clay)clay).Rebuilt(clayOptions);
        Assert.Equal("Furion", clay.name);
        Assert.Equal(clayOptions, clay.Options);

        const string keyValueJson = """
                                    [
                                      {
                                        "key": "id",
                                        "value": 1
                                      },
                                      {
                                        "key": "name",
                                        "value": "Furion"
                                      }
                                    ]
                                    """;
        var clay2 = Clay.Parse(keyValueJson);
        Assert.Equal("[{\"key\":\"id\",\"value\":1},{\"key\":\"name\",\"value\":\"Furion\"}]",
            clay2.ToJsonString());
        clay2.Rebuilt(clayOptions.Configure(u => u.KeyValueJsonToObject = true));
        Assert.Equal("{\"id\":1,\"name\":\"Furion\"}", clay2.ToJsonString());

        var clay3 = Clay.Parse(keyValueJson);
        Assert.Equal("[{\"key\":\"id\",\"value\":1},{\"key\":\"name\",\"value\":\"Furion\"}]",
            clay3.ToJsonString());
        clay3.Rebuilt(u => u.KeyValueJsonToObject = true);
        Assert.Equal("{\"id\":1,\"name\":\"Furion\"}", clay3.ToJsonString());
    }

    [Fact]
    public void Extend_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.Throws<ArgumentNullException>(() => clay.Extend(null!));

        var exception = Assert.Throws<InvalidOperationException>(() => clay.Extend([null]));
        Assert.Equal("Cannot extend a single object with null or basic type values.", exception.Message);

        var exception2 = Assert.Throws<InvalidOperationException>(() => clay.Extend(1));
        Assert.Equal("Cannot extend a single object with null or basic type values.", exception2.Message);

        var exception3 = Assert.Throws<InvalidOperationException>(() => clay.Extend(true));
        Assert.Equal("Cannot extend a single object with null or basic type values.", exception3.Message);

        var exception4 = Assert.Throws<InvalidOperationException>(() => clay.Extend("furion"));
        Assert.Equal("Cannot extend a single object with null or basic type values.", exception4.Message);
    }

    [Fact]
    public void Extend_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        clay.Extend(new { age = 30, name = "shapeless" });
        Assert.Equal("{\"id\":1,\"name\":\"shapeless\",\"age\":30}", clay.ToJsonString());

        var clay2 = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        clay2.Extend(Clay.Parse(new { age = 30, name = "shapeless" }));
        Assert.Equal("{\"id\":1,\"name\":\"shapeless\",\"age\":30}", clay2.ToJsonString());

        var clay3 = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        clay3.Extend(new Dictionary<string, object> { { "age", 30 }, { "name", "shapeless" } });
        Assert.Equal("{\"id\":1,\"name\":\"shapeless\",\"age\":30}", clay3.ToJsonString());

        var clay4 = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        clay4.Extend(new KeyValuePair<string, object>("age", 30),
            new KeyValuePair<string, object>("name", "shapeless"));
        Assert.Equal("{\"id\":1,\"name\":\"shapeless\",\"age\":30}", clay4.ToJsonString());

        var clay5 = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        clay5.Extend(ClayType.Object);
        Assert.Equal("{\"id\":1,\"name\":\"furion\",\"Object\":0}", clay5.ToJsonString());

        var array = Clay.Parse("[1,2,3]");
        array.Extend(null, "10", clay).Extend(true, new object[] { 1, 2, 3 });
        Assert.Equal("[1,2,3,null,\"10\",{\"id\":1,\"name\":\"shapeless\",\"age\":30},true,[1,2,3]]",
            array.ToJsonString());
    }

    [Theory]
    [InlineData(null, false, false)]
    [InlineData("", false, false)]
    [InlineData("  ", false, false)]
    [InlineData("""["id":1,"name":"Furion"}""", false, false)]
    [InlineData("""{"id":1,"name":"Furion"]""", false, false)]
    [InlineData("""{id:1,name:"Furion"}""", false, false)]
    [InlineData("""{'id':1,'name':"Furion"}""", false, false)]
    [InlineData("""{"id":1,"name":"Furion"}""", true, false)]
    [InlineData("""{"id":1,"name":"Furion",}""", false, false)]
    [InlineData("""{"id":1,"name":"Furion",}""", true, true)]
    [InlineData("[1,2,3]", true, false)]
    [InlineData("[1,2,3,]", false, false)]
    [InlineData("[1,2,3,]", true, true)]
    [InlineData("1,2,3]", false, false)]
    [InlineData("[1,2,3", false, false)]
    public void IsJsonObjectOrArray_ReturnOK(string? input, bool result, bool allowTrailingCommas = false) =>
        Assert.Equal(result, Clay.IsJsonObjectOrArray(input, allowTrailingCommas));

    [Fact]
    public void Deconstruct_ReturnOK()
    {
        var (clay, enumerable) = Clay.Parse("""{"id":1,"name":"furion"}""");
        Assert.Same(clay, enumerable);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay.ToJsonString());
        Assert.Equal(["id", "name"], enumerable.ToDictionary(u => u?.Key!, u => u?.Value).Keys);

        var (clay2, enumerable2, rawClay) = Clay.Parse("""{"id":1,"name":"furion"}""");
        Assert.Same(clay2, enumerable2);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay2.ToJsonString());
        Assert.Equal(["id", "name"], enumerable2.ToDictionary(u => u?.Key!, u => u?.Value).Keys);
        Assert.Equal(["id", "name"], rawClay.Keys);
    }

    [Fact]
    public void Pipe_Invalid_Parameters()
    {
        var clay = Clay.Parse("""{"id":1,"name":"furion"}""");
        Assert.Throws<ArgumentNullException>(() => clay.Pipe(null!));

        var exception = Assert.Throws<InvalidOperationException>(() => clay.Pipe(u => u.name));
        Assert.Equal(
            "An unexpected error occurred during the transformation. Please verify the implementation of the transformation function.",
            exception.Message);
        Assert.Equal(
            "Transformation must return a non-null Clay object. The provided function either returned null or an incompatible type.",
            exception.InnerException?.Message);
    }

    [Fact]
    public void Pipe_ReturnOK()
    {
        const string jsonString = """
                                  {
                                    "code": 200,
                                    "name": "toutiao",
                                    "title": "今日头条",
                                    "type": "热榜",
                                    "link": "https://www.toutiao.com/",
                                    "total": 50,
                                    "fromCache": true,
                                    "updateTime": "2025-03-08T15:20:26.252Z",
                                    "data": [
                                      {
                                        "id": "7478865230039121961",
                                        "title": "二手小米SU7 Ultra卖到65万",
                                        "cover": "https://p3-sign.toutiaoimg.com/mosaic-legacy/2b29000036f8405561443~tplv-tt-shrink:960:540.jpeg?_iz=30575&from=sign_default&lk3s=8d617dac&x-expires=1743984000&x-signature=z5DHiXWVtpp9UUQQHuYvcW52D%2FM%3D",
                                        "timestamp": 7478865230039122000,
                                        "hot": 107679,
                                        "url": "https://www.toutiao.com/trending/7478865230039121961/",
                                        "mobileUrl": "https://api.toutiaoapi.com/feoffline/amos_land/new/html/main/index.html?topic_id=7478865230039121961"
                                      },
                                      {
                                        "id": "7478393047366025257",
                                        "title": "加拿大省长模仿特朗普签令下架美国酒",
                                        "cover": "https://p3-sign.toutiaoimg.com/mosaic-legacy/2b29000036f8405561443~tplv-tt-shrink:960:540.jpeg?_iz=30575&from=sign_default&lk3s=8d617dac&x-expires=1743984000&x-signature=z5DHiXWVtpp9UUQQHuYvcW52D%2FM%3D",
                                        "timestamp": 7478393047366025000,
                                        "hot": 97432,
                                        "url": "https://www.toutiao.com/trending/7478393047366025257/",
                                        "mobileUrl": "https://api.toutiaoapi.com/feoffline/amos_land/new/html/main/index.html?topic_id=7478393047366025257"
                                      }
                                    ]
                                  }
                                  """;
        var (_, enumerable) = Clay.Parse(jsonString).Pipe(u => u.data);
        Assert.Equal(["二手小米SU7 Ultra卖到65万", "加拿大省长模仿特朗普签令下架美国酒"], enumerable.Select(u => u!.title));
    }

    [Fact]
    public void PipeTry_Invalid_Parameters()
    {
        var clay = Clay.Parse("""{"id":1,"name":"furion"}""");
        Assert.Throws<ArgumentNullException>(() => clay.PipeTry(null!));
    }

    [Fact]
    public void PipeTry_ReturnOK()
    {
        const string jsonString = """
                                  {
                                    "code": 200,
                                    "name": "toutiao",
                                    "title": "今日头条",
                                    "type": "热榜",
                                    "link": "https://www.toutiao.com/",
                                    "total": 50,
                                    "fromCache": true,
                                    "updateTime": "2025-03-08T15:20:26.252Z",
                                    "data": [
                                      {
                                        "id": "7478865230039121961",
                                        "title": "二手小米SU7 Ultra卖到65万",
                                        "cover": "https://p3-sign.toutiaoimg.com/mosaic-legacy/2b29000036f8405561443~tplv-tt-shrink:960:540.jpeg?_iz=30575&from=sign_default&lk3s=8d617dac&x-expires=1743984000&x-signature=z5DHiXWVtpp9UUQQHuYvcW52D%2FM%3D",
                                        "timestamp": 7478865230039122000,
                                        "hot": 107679,
                                        "url": "https://www.toutiao.com/trending/7478865230039121961/",
                                        "mobileUrl": "https://api.toutiaoapi.com/feoffline/amos_land/new/html/main/index.html?topic_id=7478865230039121961"
                                      },
                                      {
                                        "id": "7478393047366025257",
                                        "title": "加拿大省长模仿特朗普签令下架美国酒",
                                        "cover": "https://p3-sign.toutiaoimg.com/mosaic-legacy/2b29000036f8405561443~tplv-tt-shrink:960:540.jpeg?_iz=30575&from=sign_default&lk3s=8d617dac&x-expires=1743984000&x-signature=z5DHiXWVtpp9UUQQHuYvcW52D%2FM%3D",
                                        "timestamp": 7478393047366025000,
                                        "hot": 97432,
                                        "url": "https://www.toutiao.com/trending/7478393047366025257/",
                                        "mobileUrl": "https://api.toutiaoapi.com/feoffline/amos_land/new/html/main/index.html?topic_id=7478393047366025257"
                                      }
                                    ]
                                  }
                                  """;
        var (_, enumerable) = Clay.Parse(jsonString).PipeTry(u => u.data2).Pipe(u => u.data);
        Assert.Equal(["二手小米SU7 Ultra卖到65万", "加拿大省长模仿特朗普签令下架美国酒"], enumerable.Select(u => u!.title));
    }

    [Fact]
    public void UnicodeKey_ReturnOK()
    {
        dynamic clay = Clay.Parse("""
                                  {
                                      "sheets": {
                                          "检验报告": {
                                              "validations": "验证通过"
                                          }
                                      }
                                  }
                                  """);

        Assert.Equal("验证通过", clay.sheets.检验报告.validations);
        Assert.Equal("验证通过", clay.PathValue("sheets:检验报告:validations"));
    }

    [Fact]
    public void RemovePathValue_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.Throws<ArgumentNullException>(() => clay.RemovePathValue(null!));
        Assert.Throws<KeyNotFoundException>(() => clay.RemovePathValue("age"));
        var exception = Assert.Throws<InvalidOperationException>(() => clay.RemovePathValue("name:firstName"));
        Assert.Equal("The identifier `name` at path `name:firstName` does not support further lookup.",
            exception.Message);

        var clay2 = Clay.Parse("{\"id\":1,\"name\":\"furion\",\"children\":{\"id\":1,\"name\":\"furion\"}}");
        var exception2 =
            Assert.Throws<InvalidOperationException>(() => clay2.RemovePathValue("children:name:firstName"));
        Assert.Equal("The identifier `name` at path `name:firstName` does not support further lookup.",
            exception2.Message);
    }

    [Fact]
    public void RemovePathValue_ReturnOK()
    {
        var clay = Clay.Parse("""
                              {
                                "AppInfo": {
                                  "Name": "Furion",
                                  "Version": "1.0.0",
                                  "Company": {
                                    "Name": "Baiqian",
                                    "Address": {
                                      "City": "中国",
                                      "Province": "广东省",
                                      "Detail": "中山市东区紫马公园西门"
                                    },
                                    "Telephones":["0760-88888888","0760-88888881"],
                                    "Date":"2024-12-26T00:00:00"
                                  }
                                }
                              }
                              """);
        Assert.True(clay.RemovePathValue("AppInfo:Name"));
        Assert.Equal(
            "{\"AppInfo\":{\"Version\":\"1.0.0\",\"Company\":{\"Name\":\"Baiqian\",\"Address\":{\"City\":\"中国\",\"Province\":\"广东省\",\"Detail\":\"中山市东区紫马公园西门\"},\"Telephones\":[\"0760-88888888\",\"0760-88888881\"],\"Date\":\"2024-12-26T00:00:00\"}}}",
            clay.ToJsonString());

        Assert.True(clay.RemovePathValue("AppInfo:Company:Address:City"));
        Assert.Equal(
            "{\"AppInfo\":{\"Version\":\"1.0.0\",\"Company\":{\"Name\":\"Baiqian\",\"Address\":{\"Province\":\"广东省\",\"Detail\":\"中山市东区紫马公园西门\"},\"Telephones\":[\"0760-88888888\",\"0760-88888881\"],\"Date\":\"2024-12-26T00:00:00\"}}}",
            clay.ToJsonString());

        Assert.True(clay.RemovePathValue("AppInfo:Company:Telephones:1"));
        Assert.Equal(
            "{\"AppInfo\":{\"Version\":\"1.0.0\",\"Company\":{\"Name\":\"Baiqian\",\"Address\":{\"Province\":\"广东省\",\"Detail\":\"中山市东区紫马公园西门\"},\"Telephones\":[\"0760-88888888\"],\"Date\":\"2024-12-26T00:00:00\"}}}",
            clay.ToJsonString());

        Assert.True(clay.RemovePathValue("AppInfo:Company:Date"));
        Assert.Equal(
            "{\"AppInfo\":{\"Version\":\"1.0.0\",\"Company\":{\"Name\":\"Baiqian\",\"Address\":{\"Province\":\"广东省\",\"Detail\":\"中山市东区紫马公园西门\"},\"Telephones\":[\"0760-88888888\"]}}}",
            clay.ToJsonString());

        clay.Rebuilt(ClayOptions.Flexible);
        Assert.False(clay.RemovePathValue("AppInfo:Undefined"));

        var array = Clay.Parse("""
                               [0,
                               {"id":1,"name":"Furion"},
                               {
                                 "AppInfo": {
                                   "Name": "Furion",
                                   "Version": "1.0.0",
                                   "Company": {
                                     "Name": "Baiqian",
                                     "Address": {
                                       "City": "中国",
                                       "Province": "广东省",
                                       "Detail": "中山市东区紫马公园西门"
                                     },
                                     "Telephones":["0760-88888888","0760-88888881"],
                                     "Date":"2024-12-26T00:00:00"
                                   }
                                 }
                               }]
                               """);
        Assert.True(array.RemovePathValue("1:name"));
        Assert.True(array.RemovePathValue("2:AppInfo:Company:Address:City"));
        Assert.Equal(
            "[0,{\"id\":1},{\"AppInfo\":{\"Name\":\"Furion\",\"Version\":\"1.0.0\",\"Company\":{\"Name\":\"Baiqian\",\"Address\":{\"Province\":\"广东省\",\"Detail\":\"中山市东区紫马公园西门\"},\"Telephones\":[\"0760-88888888\",\"0760-88888881\"],\"Date\":\"2024-12-26T00:00:00\"}}}]",
            array.ToJsonString());

        var clay2 = Clay.Parse("""
                               {
                                 "AppInfo": {
                                   "Name": "Furion",
                                   "Version": "1.0.0",
                                   "Company": {
                                     "Name": "Baiqian",
                                     "Address": {
                                       "City": "中国",
                                       "Province": "广东省",
                                       "Detail": "中山市东区紫马公园西门"
                                     },
                                     "Telephones":["0760-88888888","0760-88888881"],
                                     "Date":"2024-12-26T00:00:00"
                                   }
                                 }
                               }
                               """, new ClayOptions { PathSeparator = [":", "/"] });
        Assert.True(clay2.RemovePathValue("AppInfo:Name"));
        Assert.True(clay2.RemovePathValue("AppInfo:Company/Address:City"));
        Assert.True(clay2.RemovePathValue("AppInfo/Company/Telephones/1"));
        Assert.Equal(
            "{\"AppInfo\":{\"Version\":\"1.0.0\",\"Company\":{\"Name\":\"Baiqian\",\"Address\":{\"Province\":\"广东省\",\"Detail\":\"中山市东区紫马公园西门\"},\"Telephones\":[\"0760-88888888\"],\"Date\":\"2024-12-26T00:00:00\"}}}",
            clay2.ToJsonString());
    }

    [Fact]
    public void DeletePathValue_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.Throws<ArgumentNullException>(() => clay.DeletePathValue(null!));
        Assert.Throws<KeyNotFoundException>(() => clay.DeletePathValue("age"));
        var exception = Assert.Throws<InvalidOperationException>(() => clay.DeletePathValue("name:firstName"));
        Assert.Equal("The identifier `name` at path `name:firstName` does not support further lookup.",
            exception.Message);

        var clay2 = Clay.Parse("{\"id\":1,\"name\":\"furion\",\"children\":{\"id\":1,\"name\":\"furion\"}}");
        var exception2 =
            Assert.Throws<InvalidOperationException>(() => clay2.DeletePathValue("children:name:firstName"));
        Assert.Equal("The identifier `name` at path `name:firstName` does not support further lookup.",
            exception2.Message);
    }

    [Fact]
    public void DeletePathValue_ReturnOK()
    {
        var clay = Clay.Parse("""
                              {
                                "AppInfo": {
                                  "Name": "Furion",
                                  "Version": "1.0.0",
                                  "Company": {
                                    "Name": "Baiqian",
                                    "Address": {
                                      "City": "中国",
                                      "Province": "广东省",
                                      "Detail": "中山市东区紫马公园西门"
                                    },
                                    "Telephones":["0760-88888888","0760-88888881"],
                                    "Date":"2024-12-26T00:00:00"
                                  }
                                }
                              }
                              """);
        Assert.True(clay.DeletePathValue("AppInfo:Name"));
        Assert.Equal(
            "{\"AppInfo\":{\"Version\":\"1.0.0\",\"Company\":{\"Name\":\"Baiqian\",\"Address\":{\"City\":\"中国\",\"Province\":\"广东省\",\"Detail\":\"中山市东区紫马公园西门\"},\"Telephones\":[\"0760-88888888\",\"0760-88888881\"],\"Date\":\"2024-12-26T00:00:00\"}}}",
            clay.ToJsonString());

        Assert.True(clay.DeletePathValue("AppInfo:Company:Address:City"));
        Assert.Equal(
            "{\"AppInfo\":{\"Version\":\"1.0.0\",\"Company\":{\"Name\":\"Baiqian\",\"Address\":{\"Province\":\"广东省\",\"Detail\":\"中山市东区紫马公园西门\"},\"Telephones\":[\"0760-88888888\",\"0760-88888881\"],\"Date\":\"2024-12-26T00:00:00\"}}}",
            clay.ToJsonString());

        Assert.True(clay.DeletePathValue("AppInfo:Company:Telephones:1"));
        Assert.Equal(
            "{\"AppInfo\":{\"Version\":\"1.0.0\",\"Company\":{\"Name\":\"Baiqian\",\"Address\":{\"Province\":\"广东省\",\"Detail\":\"中山市东区紫马公园西门\"},\"Telephones\":[\"0760-88888888\"],\"Date\":\"2024-12-26T00:00:00\"}}}",
            clay.ToJsonString());

        Assert.True(clay.DeletePathValue("AppInfo:Company:Date"));
        Assert.Equal(
            "{\"AppInfo\":{\"Version\":\"1.0.0\",\"Company\":{\"Name\":\"Baiqian\",\"Address\":{\"Province\":\"广东省\",\"Detail\":\"中山市东区紫马公园西门\"},\"Telephones\":[\"0760-88888888\"]}}}",
            clay.ToJsonString());

        clay.Rebuilt(ClayOptions.Flexible);
        Assert.False(clay.DeletePathValue("AppInfo:Undefined"));

        var array = Clay.Parse("""
                               [0,
                               {"id":1,"name":"Furion"},
                               {
                                 "AppInfo": {
                                   "Name": "Furion",
                                   "Version": "1.0.0",
                                   "Company": {
                                     "Name": "Baiqian",
                                     "Address": {
                                       "City": "中国",
                                       "Province": "广东省",
                                       "Detail": "中山市东区紫马公园西门"
                                     },
                                     "Telephones":["0760-88888888","0760-88888881"],
                                     "Date":"2024-12-26T00:00:00"
                                   }
                                 }
                               }]
                               """);
        Assert.True(array.DeletePathValue("1:name"));
        Assert.True(array.DeletePathValue("2:AppInfo:Company:Address:City"));
        Assert.Equal(
            "[0,{\"id\":1},{\"AppInfo\":{\"Name\":\"Furion\",\"Version\":\"1.0.0\",\"Company\":{\"Name\":\"Baiqian\",\"Address\":{\"Province\":\"广东省\",\"Detail\":\"中山市东区紫马公园西门\"},\"Telephones\":[\"0760-88888888\",\"0760-88888881\"],\"Date\":\"2024-12-26T00:00:00\"}}}]",
            array.ToJsonString());

        var clay2 = Clay.Parse("""
                               {
                                 "AppInfo": {
                                   "Name": "Furion",
                                   "Version": "1.0.0",
                                   "Company": {
                                     "Name": "Baiqian",
                                     "Address": {
                                       "City": "中国",
                                       "Province": "广东省",
                                       "Detail": "中山市东区紫马公园西门"
                                     },
                                     "Telephones":["0760-88888888","0760-88888881"],
                                     "Date":"2024-12-26T00:00:00"
                                   }
                                 }
                               }
                               """, new ClayOptions { PathSeparator = [":", "/"] });
        Assert.True(clay2.DeletePathValue("AppInfo:Name"));
        Assert.True(clay2.DeletePathValue("AppInfo:Company/Address:City"));
        Assert.True(clay2.DeletePathValue("AppInfo/Company/Telephones/1"));
        Assert.Equal(
            "{\"AppInfo\":{\"Version\":\"1.0.0\",\"Company\":{\"Name\":\"Baiqian\",\"Address\":{\"Province\":\"广东省\",\"Detail\":\"中山市东区紫马公园西门\"},\"Telephones\":[\"0760-88888888\"],\"Date\":\"2024-12-26T00:00:00\"}}}",
            clay2.ToJsonString());
    }

    [Fact]
    public void SetPathValue_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.Throws<ArgumentNullException>(() => clay.SetPathValue(null!, null));
        Assert.Throws<KeyNotFoundException>(() => clay.SetPathValue("age", null));
        var exception = Assert.Throws<InvalidOperationException>(() => clay.SetPathValue("name:firstName", null));
        Assert.Equal("The identifier `name` at path `name:firstName` does not support further lookup.",
            exception.Message);

        var clay2 = Clay.Parse("{\"id\":1,\"name\":\"furion\",\"children\":{\"id\":1,\"name\":\"furion\"}}");
        var exception2 =
            Assert.Throws<InvalidOperationException>(() => clay2.SetPathValue("children:name:firstName", null));
        Assert.Equal("The identifier `name` at path `name:firstName` does not support further lookup.",
            exception2.Message);
    }

    [Fact]
    public void SetPathValue_ReturnOK()
    {
        var clay = Clay.Parse("""
                              {
                                "AppInfo": {
                                  "Name": "Furion",
                                  "Version": "1.0.0",
                                  "Company": {
                                    "Name": "Baiqian",
                                    "Address": {
                                      "City": "中国",
                                      "Province": "广东省",
                                      "Detail": "中山市东区紫马公园西门"
                                    },
                                    "Telephones":["0760-88888888","0760-88888881"],
                                    "Date":"2024-12-26T00:00:00"
                                  }
                                }
                              }
                              """);
        clay.SetPathValue("AppInfo:Name", "百小僧");
        Assert.Equal("百小僧", clay["AppInfo"]!["Name"]);

        clay.SetPathValue("AppInfo:Company:Address:City", "中国中山市");
        Assert.Equal("中国中山市", clay["AppInfo"]!["Company"]["Address"]["City"]);

        clay.SetPathValue("AppInfo:Company:Telephones:0", "0760-88888882");
        Assert.Equal("0760-88888882", clay["AppInfo"]!["Company"]["Telephones"][0]);

        clay.Rebuilt(ClayOptions.Flexible);
        clay.SetPathValue("AppInfo:Undefined", "Data");
        Assert.Null(clay["AppInfo"]!["Undefined"]);

        var array = Clay.Parse("""
                               [0,
                               {"id":1,"name":"Furion"},
                               {
                                 "AppInfo": {
                                   "Name": "Furion",
                                   "Version": "1.0.0",
                                   "Company": {
                                     "Name": "Baiqian",
                                     "Address": {
                                       "City": "中国",
                                       "Province": "广东省",
                                       "Detail": "中山市东区紫马公园西门"
                                     },
                                     "Telephones":["0760-88888888","0760-88888881"],
                                     "Date":"2024-12-26T00:00:00"
                                   }
                                 }
                               }]
                               """);
        array.SetPathValue("1:name", "百小僧");
        Assert.Equal("百小僧", array[1]!["name"]);
        array.SetPathValue("2:AppInfo:Company:Address:City", "中国中山市");
        Assert.Equal("中国中山市", array[2]!["AppInfo"]["Company"]["Address"]["City"]);

        var clay2 = Clay.Parse("""
                               {
                                 "AppInfo": {
                                   "Name": "Furion",
                                   "Version": "1.0.0",
                                   "Company": {
                                     "Name": "Baiqian",
                                     "Address": {
                                       "City": "中国",
                                       "Province": "广东省",
                                       "Detail": "中山市东区紫马公园西门"
                                     },
                                     "Telephones":["0760-88888888","0760-88888881"],
                                     "Date":"2024-12-26T00:00:00"
                                   }
                                 }
                               }
                               """, new ClayOptions { PathSeparator = [":", "/"] });
        clay2.SetPathValue("AppInfo:Name", "百小僧");
        Assert.Equal("百小僧", clay2["AppInfo"]!["Name"]);
        clay2.SetPathValue("AppInfo:Company/Address:City", "中国中山市");
        Assert.Equal("中国中山市", clay2["AppInfo"]!["Company"]["Address"]["City"]);
        clay2.SetPathValue("AppInfo/Company/Telephones/0", "0760-88888882");
        Assert.Equal("0760-88888882", clay2["AppInfo"]!["Company"]["Telephones"][0]);
    }

    [Fact]
    public void NestedDictionary_ReturnOK()
    {
        dynamic clay = new Clay();
        clay.one = new Dictionary<string, object> { { "one1", "value" }, { "one2", "value2" } };
        clay.two = new Dictionary<string, object> { { "two1", "value" }, { "two2", "value2" } };

        Dictionary<string, Dictionary<string, object>> dic = clay;
        Assert.Equal(2, dic.Count);

        Assert.True(dic.ContainsKey("one"));
        Assert.True(dic["one"].ContainsKey("one1"));
        Assert.True(dic["one"].ContainsKey("one2"));

        Assert.True(dic.ContainsKey("two"));
        Assert.True(dic["two"].ContainsKey("two1"));
        Assert.True(dic["two"].ContainsKey("two2"));
    }

    /// <summary>
    ///     生成嵌套 JSON 字符串
    /// </summary>
    /// <param name="depth">嵌套层级</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    private static string BuildNestedJson(int depth)
    {
        var current = """{"value": "final"}""";
        for (var i = depth; i >= 1; i--)
        {
            var escaped = current.Replace("\\", @"\\").Replace("\"", "\\\"");
            current = $"{{\"level{i}\": \"{escaped}\"}}";
        }

        return current;
    }
}

public struct Point
{
    public int X;
    public int Y;
}

public struct Point2
{
    [JsonInclude] public int X;
    [JsonInclude] public int Y;
}

public class StringClassTest
{
    public string? String1 { get; set; }
    public string? String2 { get; set; }
    public string? String3 { get; set; }
}

public class UnixEpochDateClass
{
    public DateTime DateTime { get; set; }
    public DateTimeOffset DateTimeOffset { get; set; }
}

public class CustomClass<T>
    where T : class, new()
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public IList<T>? Items { get; set; }
}