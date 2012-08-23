using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ChronoPlurk.Views.ImageLoader
{
    public static class EmoticonsDictionary
    {
        private static Dictionary<string, Uri> _dictionary;

        public static bool Contains(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public static bool TryGetValue(string key, out Uri value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public static void Initialize()
        {
            // Official (girlkiss) file is corrupted GIF so using converted version.
            _dictionary = new Dictionary<string, Uri>(125)
            {
{"http://statics.plurk.com/ff124032f8cc3a9d43b99e661f8fcb4d.gif", new Uri("ff124032f8cc3a9d43b99e661f8fcb4d.png", UriKind.Relative)},
{"http://statics.plurk.com/99ef3957ef779718546752b749bdeabd.gif", new Uri("99ef3957ef779718546752b749bdeabd.png", UriKind.Relative)},
{"http://statics.plurk.com/3385896779bf1c13188bf92ca516878e.gif", new Uri("3385896779bf1c13188bf92ca516878e.png", UriKind.Relative)},
{"http://statics.plurk.com/615f18f7ea8abc608c4c20eaa667883b.gif", new Uri("615f18f7ea8abc608c4c20eaa667883b.png", UriKind.Relative)},
{"http://statics.plurk.com/2d5e21929e752498e36d74096b1965e1.gif", new Uri("2d5e21929e752498e36d74096b1965e1.png", UriKind.Relative)},
{"http://statics.plurk.com/13b15aa49358be8f47b58552401d7725.gif", new Uri("13b15aa49358be8f47b58552401d7725.png", UriKind.Relative)},
{"http://statics.plurk.com/57c69f50e40a283dcd2e7b56fc191abe.gif", new Uri("57c69f50e40a283dcd2e7b56fc191abe.png", UriKind.Relative)},
{"http://statics.plurk.com/8eb05ca7a32301ba16c9496bcad893c4.gif", new Uri("8eb05ca7a32301ba16c9496bcad893c4.png", UriKind.Relative)},
{"http://statics.plurk.com/261c0fe4a88417146ae0292d697a5f52.gif", new Uri("261c0fe4a88417146ae0292d697a5f52.png", UriKind.Relative)},
{"http://statics.plurk.com/11eed61b41a3e935773476ac33bc00d9.gif", new Uri("11eed61b41a3e935773476ac33bc00d9.png", UriKind.Relative)},
{"http://statics.plurk.com/72ddf2c585fe77dd0be731b19624d8cb.gif", new Uri("72ddf2c585fe77dd0be731b19624d8cb.png", UriKind.Relative)},
{"http://statics.plurk.com/2884b8d0e496c06136c86e9c9599edae.gif", new Uri("2884b8d0e496c06136c86e9c9599edae.png", UriKind.Relative)},
{"http://statics.plurk.com/1a5f23ed863e70e52f239b045a48e6fb.gif", new Uri("1a5f23ed863e70e52f239b045a48e6fb.png", UriKind.Relative)},
{"http://statics.plurk.com/f5dbd5fdf5f5df69cfb024d6be76a76b.gif", new Uri("f5dbd5fdf5f5df69cfb024d6be76a76b.png", UriKind.Relative)},
{"http://statics.plurk.com/e902170e97aee14836b5df6b0fe61ba2.gif", new Uri("e902170e97aee14836b5df6b0fe61ba2.png", UriKind.Relative)},
{"http://statics.plurk.com/e476574723d5042f24658fa36866bd92.gif", new Uri("e476574723d5042f24658fa36866bd92.png", UriKind.Relative)},
{"http://statics.plurk.com/7256dae81d56d150120ccd0c96dd2197.gif", new Uri("7256dae81d56d150120ccd0c96dd2197.png", UriKind.Relative)},
{"http://statics.plurk.com/deda4d9f78ad528d725e3a6bfbf6352f.gif", new Uri("deda4d9f78ad528d725e3a6bfbf6352f.png", UriKind.Relative)},
{"http://statics.plurk.com/47d20905d017c396d67b4a30c9ac9b10.png", new Uri("47d20905d017c396d67b4a30c9ac9b10.png", UriKind.Relative)},
{"http://statics.plurk.com/0efc4d55d28704f4370ef874ae906161.gif", new Uri("0efc4d55d28704f4370ef874ae906161.png", UriKind.Relative)},
{"http://statics.plurk.com/4ad099fba019942f13058610ff3fc568.gif", new Uri("4ad099fba019942f13058610ff3fc568.png", UriKind.Relative)},
{"http://statics.plurk.com/5a2a63fa773e68797ec69a1303bfa3b9.png", new Uri("5a2a63fa773e68797ec69a1303bfa3b9.png", UriKind.Relative)},
{"http://statics.plurk.com/4c40d16a0d369b895c08f2e33d062ec8.gif", new Uri("4c40d16a0d369b895c08f2e33d062ec8.png", UriKind.Relative)},
{"http://statics.plurk.com/3acbaf42504fff32c5eac4f12083ce56.gif", new Uri("3acbaf42504fff32c5eac4f12083ce56.png", UriKind.Relative)},
{"http://statics.plurk.com/fcd28d7d78ec1f828c76930fa63270e6.gif", new Uri("fcd28d7d78ec1f828c76930fa63270e6.png", UriKind.Relative)},
{"http://statics.plurk.com/71acd802cc931649dd9a371ccf70bad2.gif", new Uri("71acd802cc931649dd9a371ccf70bad2.png", UriKind.Relative)},
{"http://statics.plurk.com/bb1e3fed482959a00013f7f1efcc17a0.gif", new Uri("bb1e3fed482959a00013f7f1efcc17a0.png", UriKind.Relative)},
{"http://statics.plurk.com/bac8c8392f7ca8f5ac74612be4d08b74.gif", new Uri("bac8c8392f7ca8f5ac74612be4d08b74.png", UriKind.Relative)},
{"http://statics.plurk.com/a555399b40c379adca5b6f5bad5bf732.gif", new Uri("a555399b40c379adca5b6f5bad5bf732.png", UriKind.Relative)},
{"http://statics.plurk.com/8855f56400a936db07f348d9290adaac.gif", new Uri("8855f56400a936db07f348d9290adaac.png", UriKind.Relative)},
{"http://statics.plurk.com/6675254cd7449b1847a93b0024127eae.gif", new Uri("6675254cd7449b1847a93b0024127eae.png", UriKind.Relative)},
{"http://statics.plurk.com/5b51892d7d1f392d93ea7fe26e5100f4.gif", new Uri("5b51892d7d1f392d93ea7fe26e5100f4.png", UriKind.Relative)},
{"http://statics.plurk.com/6de58c967f1c2797d250a713ba50eddd.gif", new Uri("6de58c967f1c2797d250a713ba50eddd.png", UriKind.Relative)},
{"http://statics.plurk.com/feb43dbbbf2763905571060be9a496d1.gif", new Uri("feb43dbbbf2763905571060be9a496d1.png", UriKind.Relative)},
{"http://statics.plurk.com/88fac5a4b99110a35d4e4794dad58ab4.gif", new Uri("88fac5a4b99110a35d4e4794dad58ab4.png", UriKind.Relative)},
{"http://statics.plurk.com/b3b9856e557fcc2700fd41c53f9d4910.gif", new Uri("b3b9856e557fcc2700fd41c53f9d4910.png", UriKind.Relative)},
{"http://statics.plurk.com/cfdd2accc1188f5fbc62e149074c7f29.png", new Uri("cfdd2accc1188f5fbc62e149074c7f29.png", UriKind.Relative)},
{"http://statics.plurk.com/828b9819249db696701ae0987fba3638.png", new Uri("828b9819249db696701ae0987fba3638.png", UriKind.Relative)},
{"http://statics.plurk.com/1bd653e166492e40e214ef6ce4dd716f.png", new Uri("1bd653e166492e40e214ef6ce4dd716f.png", UriKind.Relative)},
{"http://statics.plurk.com/9c5c54081547d2ad903648f178fcc595.png", new Uri("9c5c54081547d2ad903648f178fcc595.png", UriKind.Relative)},
{"http://statics.plurk.com/3fe6cf919158597d7ec74f8d90f0cc9f.png", new Uri("3fe6cf919158597d7ec74f8d90f0cc9f.png", UriKind.Relative)},
{"http://statics.plurk.com/2da76999ca3716fb4053f3332270e5c9.png", new Uri("2da76999ca3716fb4053f3332270e5c9.png", UriKind.Relative)},
{"http://statics.plurk.com/f73b773aa689647cb09f57f67a83bb89.png", new Uri("f73b773aa689647cb09f57f67a83bb89.png", UriKind.Relative)},
{"http://statics.plurk.com/45beda260eddc28c82c0d27377e7bf42.png", new Uri("45beda260eddc28c82c0d27377e7bf42.png", UriKind.Relative)},
{"http://statics.plurk.com/8590888362ae83daed52e4ca73c296a6.png", new Uri("8590888362ae83daed52e4ca73c296a6.png", UriKind.Relative)},
{"http://statics.plurk.com/c7551098438cc28ec3b54281d4b09cc3.png", new Uri("c7551098438cc28ec3b54281d4b09cc3.png", UriKind.Relative)},
{"http://statics.plurk.com/cfd84315ebceec0c4389c51cf69132bd.png", new Uri("cfd84315ebceec0c4389c51cf69132bd.png", UriKind.Relative)},
{"http://statics.plurk.com/0e0bf1ec2c2958799666f3995ef830ca.png", new Uri("0e0bf1ec2c2958799666f3995ef830ca.png", UriKind.Relative)},
{"http://statics.plurk.com/e2998ca75f80c1c4a5508c549e3980a6.png", new Uri("e2998ca75f80c1c4a5508c549e3980a6.png", UriKind.Relative)},
{"http://statics.plurk.com/c6ad1c4f9e11f6859a1ba39c4341ef8b.png", new Uri("c6ad1c4f9e11f6859a1ba39c4341ef8b.png", UriKind.Relative)},
{"http://statics.plurk.com/4a61085f1c6a639f028cd48ae97d07d0.png", new Uri("4a61085f1c6a639f028cd48ae97d07d0.png", UriKind.Relative)},
{"http://statics.plurk.com/53253ca60f5831f0812954213a2e9bb3.png", new Uri("53253ca60f5831f0812954213a2e9bb3.png", UriKind.Relative)},
{"http://statics.plurk.com/6928f3117658cc38d94e70519a511005.png", new Uri("6928f3117658cc38d94e70519a511005.png", UriKind.Relative)},
// {"http://statics.plurk.com/2678bdb36b318b038a3a1a9e7fdb2220.png", new Uri("2678bdb36b318b038a3a1a9e7fdb2220.png", UriKind.Relative)}, Ignore ugly pic
{"http://statics.plurk.com/fe2398c09a67a416f16353af91283bd0.png", new Uri("fe2398c09a67a416f16353af91283bd0.png", UriKind.Relative)},
{"http://statics.plurk.com/dd8468c4e7af6c57e3b176a8c984fc7d.png", new Uri("dd8468c4e7af6c57e3b176a8c984fc7d.png", UriKind.Relative)},
{"http://statics.plurk.com/7f42645feb6ceed6e35637eaf418306c.png", new Uri("7f42645feb6ceed6e35637eaf418306c.png", UriKind.Relative)},
{"http://statics.plurk.com/3d38ab77e8df38579df2403d382d63dd.png", new Uri("3d38ab77e8df38579df2403d382d63dd.png", UriKind.Relative)},
{"http://statics.plurk.com/b82e3225c92a764d225429a6487d9f04.gif", new Uri("b82e3225c92a764d225429a6487d9f04.png", UriKind.Relative)},
{"http://statics.plurk.com/9454d15bcaf411b159dcc147ebc3f0eb.gif", new Uri("9454d15bcaf411b159dcc147ebc3f0eb.png", UriKind.Relative)},
{"http://statics.plurk.com/a5ae31c4185bc60cd006650dc10f8147.gif", new Uri("a5ae31c4185bc60cd006650dc10f8147.png", UriKind.Relative)},
{"http://statics.plurk.com/35b16fc25623670e41c2be6bf8ac38c7.gif", new Uri("35b16fc25623670e41c2be6bf8ac38c7.png", UriKind.Relative)},
{"http://statics.plurk.com/4afd784c0df9f7a3ceacb92beca543f6.gif", new Uri("4afd784c0df9f7a3ceacb92beca543f6.png", UriKind.Relative)},
{"http://statics.plurk.com/c1c9870cf653fa3cd103d2eb0f519ccb.gif", new Uri("c1c9870cf653fa3cd103d2eb0f519ccb.png", UriKind.Relative)},
{"http://statics.plurk.com/d1a6f08507b126ec6a215e6a2372e8bb.gif", new Uri("d1a6f08507b126ec6a215e6a2372e8bb.png", UriKind.Relative)},
{"http://statics.plurk.com/5495d64ccb898ca596b061168fa0374a.gif", new Uri("5495d64ccb898ca596b061168fa0374a.png", UriKind.Relative)},
{"http://statics.plurk.com/65271ac2126706bc09d31ff67c525d67.gif", new Uri("65271ac2126706bc09d31ff67c525d67.png", UriKind.Relative)},
{"http://statics.plurk.com/a709dab8ddd26bd222466d31bd549579.png", new Uri("a709dab8ddd26bd222466d31bd549579.png", UriKind.Relative)},
{"http://statics.plurk.com/e3baa9d0d78c35e955a6b07c39f530fa.gif", new Uri("e3baa9d0d78c35e955a6b07c39f530fa.png", UriKind.Relative)},
{"http://statics.plurk.com/0f96595ed7733393b93a3d67aa4f2f4f.gif", new Uri("0f96595ed7733393b93a3d67aa4f2f4f.png", UriKind.Relative)},
{"http://statics.plurk.com/919b87048fdf7bd59dae457f4284b20b.gif", new Uri("919b87048fdf7bd59dae457f4284b20b.png", UriKind.Relative)},
{"http://statics.plurk.com/96872d481bbfe87aad5aed976c7de4ee.gif", new Uri("96872d481bbfe87aad5aed976c7de4ee.png", UriKind.Relative)},
{"http://statics.plurk.com/56336bb821c4766001816639e55e5811.gif", new Uri("56336bb821c4766001816639e55e5811.png", UriKind.Relative)},
{"http://statics.plurk.com/6cb1dc388b9259565efedef8f336d27d.gif", new Uri("6cb1dc388b9259565efedef8f336d27d.png", UriKind.Relative)},
{"http://statics.plurk.com/a9560787e93f4f8890e4bd38696ba537.gif", new Uri("a9560787e93f4f8890e4bd38696ba537.png", UriKind.Relative)},
{"http://statics.plurk.com/a55bdb344892676b0fea545354654a49.gif", new Uri("a55bdb344892676b0fea545354654a49.png", UriKind.Relative)},
{"http://statics.plurk.com/9939dd585cf0e8d39e7912a98a9ce727.gif", new Uri("9939dd585cf0e8d39e7912a98a9ce727.png", UriKind.Relative)},
{"http://statics.plurk.com/e8ed6c7eed76d2acd9dbf469f29fbec2.gif", new Uri("e8ed6c7eed76d2acd9dbf469f29fbec2.png", UriKind.Relative)},
{"http://statics.plurk.com/2b3593aea68efa7a00b4ef2850f98b8a.gif", new Uri("2b3593aea68efa7a00b4ef2850f98b8a.png", UriKind.Relative)},
{"http://statics.plurk.com/ed3620ff28a02e3dc9ac4ffa8e6ae2e6.gif", new Uri("ed3620ff28a02e3dc9ac4ffa8e6ae2e6.png", UriKind.Relative)},
{"http://statics.plurk.com/08a43d50691a1ed22706fc92f568fa07.gif", new Uri("000_girlkiss.png", UriKind.Relative)},
{"http://statics.plurk.com/8600839dc03e6275b53fd03a0eba09cf.gif", new Uri("8600839dc03e6275b53fd03a0eba09cf.png", UriKind.Relative)},
{"http://statics.plurk.com/e6bb16b6ef386c5f23900b103dbdba31.gif", new Uri("e6bb16b6ef386c5f23900b103dbdba31.png", UriKind.Relative)},
{"http://statics.plurk.com/f053074bcce500fbd1e2327d49748a6d.gif", new Uri("f053074bcce500fbd1e2327d49748a6d.png", UriKind.Relative)},
{"http://statics.plurk.com/1f44d3984a094fceae1f1a016a730fc9.gif", new Uri("1f44d3984a094fceae1f1a016a730fc9.png", UriKind.Relative)},
{"http://statics.plurk.com/2f7c90ce56fb4a70e34c04d8d7692dd0.gif", new Uri("2f7c90ce56fb4a70e34c04d8d7692dd0.png", UriKind.Relative)},
{"http://statics.plurk.com/900f3dd0adaad9142d567caf6bfb1311.gif", new Uri("900f3dd0adaad9142d567caf6bfb1311.png", UriKind.Relative)},
{"http://statics.plurk.com/95ace5ba1097301b5206a9e0fb431624.gif", new Uri("95ace5ba1097301b5206a9e0fb431624.png", UriKind.Relative)},
{"http://statics.plurk.com/95e69aa508d4bb435706b9db0a610dad.gif", new Uri("95e69aa508d4bb435706b9db0a610dad.png", UriKind.Relative)},
{"http://statics.plurk.com/a08ed27ec14b48d4703f53f7eb94834b.gif", new Uri("a08ed27ec14b48d4703f53f7eb94834b.png", UriKind.Relative)},
{"http://statics.plurk.com/85ef5fa01a6a67a525429f8bf4279fe7.gif", new Uri("85ef5fa01a6a67a525429f8bf4279fe7.png", UriKind.Relative)},
{"http://statics.plurk.com/1c890273544559b17f090d09238fa763.gif", new Uri("1c890273544559b17f090d09238fa763.png", UriKind.Relative)},
{"http://statics.plurk.com/986ecf2b1ae69072e0195b0a58545900.gif", new Uri("986ecf2b1ae69072e0195b0a58545900.png", UriKind.Relative)},
{"http://statics.plurk.com/150e3f367a063d3baf9720719d78d778.gif", new Uri("150e3f367a063d3baf9720719d78d778.png", UriKind.Relative)},
{"http://statics.plurk.com/3fabe74e992888be701de2a9d80c180a.gif", new Uri("3fabe74e992888be701de2a9d80c180a.png", UriKind.Relative)},
{"http://statics.plurk.com/92b595a573d25dd5e39a57b5d56d4d03.gif", new Uri("92b595a573d25dd5e39a57b5d56d4d03.png", UriKind.Relative)},
{"http://statics.plurk.com/af44689f789b98cfcb103844f7fbfce8.png", new Uri("af44689f789b98cfcb103844f7fbfce8.png", UriKind.Relative)},
{"http://statics.plurk.com/84f94a47fcaf1df0a5f17a1cfa52fa64.gif", new Uri("84f94a47fcaf1df0a5f17a1cfa52fa64.png", UriKind.Relative)},
{"http://statics.plurk.com/44117848701cd748460921cfea5c3781.gif", new Uri("44117848701cd748460921cfea5c3781.png", UriKind.Relative)},
{"http://statics.plurk.com/88f6dda8d290f66a923c1116a2a2b4ab.gif", new Uri("88f6dda8d290f66a923c1116a2a2b4ab.png", UriKind.Relative)},
{"http://statics.plurk.com/eeaf87c619a0221ec9fa06413fd2d5dc.gif", new Uri("eeaf87c619a0221ec9fa06413fd2d5dc.png", UriKind.Relative)},
{"http://statics.plurk.com/48ec47723cb34be3fcbc914e591e69cd.gif", new Uri("48ec47723cb34be3fcbc914e591e69cd.png", UriKind.Relative)},
{"http://statics.plurk.com/259a728a690204148037fbee7e2ca2d3.gif", new Uri("259a728a690204148037fbee7e2ca2d3.png", UriKind.Relative)},
{"http://statics.plurk.com/4383af0e055bce112176c5104deeaadf.gif", new Uri("4383af0e055bce112176c5104deeaadf.png", UriKind.Relative)},
{"http://statics.plurk.com/70722ab5756c3b89c86d85feab91725d.gif", new Uri("70722ab5756c3b89c86d85feab91725d.png", UriKind.Relative)},
{"http://statics.plurk.com/91cf07e3aa16738943fa1147940b48ea.gif", new Uri("91cf07e3aa16738943fa1147940b48ea.png", UriKind.Relative)},
{"http://statics.plurk.com/4f01bac8a707e5450307f97065ec0fa7.gif", new Uri("4f01bac8a707e5450307f97065ec0fa7.png", UriKind.Relative)},
{"http://statics.plurk.com/48515125401120316abb97666458d05b.gif", new Uri("48515125401120316abb97666458d05b.png", UriKind.Relative)},
{"http://statics.plurk.com/aabbc82c2b0dc72bfbce2f82c97a95e8.gif", new Uri("aabbc82c2b0dc72bfbce2f82c97a95e8.png", UriKind.Relative)},
{"http://statics.plurk.com/b0b0596acce9ffc1f2a27548aa642eaf.gif", new Uri("b0b0596acce9ffc1f2a27548aa642eaf.png", UriKind.Relative)},
{"http://statics.plurk.com/52991d7ff65a05526454bd1170a0f14c.gif", new Uri("52991d7ff65a05526454bd1170a0f14c.png", UriKind.Relative)},
{"http://statics.plurk.com/846277f0a154dc95a08594b7d32a5ccd.gif", new Uri("846277f0a154dc95a08594b7d32a5ccd.png", UriKind.Relative)},
{"http://statics.plurk.com/843739a95294fd0bf4c958840b46408f.gif", new Uri("843739a95294fd0bf4c958840b46408f.png", UriKind.Relative)},
{"http://statics.plurk.com/22416dced8b59446db8cd366cc925d09.gif", new Uri("22416dced8b59446db8cd366cc925d09.png", UriKind.Relative)},
{"http://statics.plurk.com/e3f0f67ca3af62e34f13abf1d036a010.gif", new Uri("e3f0f67ca3af62e34f13abf1d036a010.png", UriKind.Relative)},
{"http://statics.plurk.com/8073c1716e75d32eb79f97a9f521fa01.gif", new Uri("8073c1716e75d32eb79f97a9f521fa01.png", UriKind.Relative)},
{"http://statics.plurk.com/373cd2f23dab7528d4875170d13d64f7.gif", new Uri("373cd2f23dab7528d4875170d13d64f7.png", UriKind.Relative)},
{"http://statics.plurk.com/8863234ebea13f109c9b15ba19a4531c.gif", new Uri("8863234ebea13f109c9b15ba19a4531c.png", UriKind.Relative)},
{"http://statics.plurk.com/8738c7a1c402f41b5319abe504ce9687.gif", new Uri("8738c7a1c402f41b5319abe504ce9687.png", UriKind.Relative)},
{"http://statics.plurk.com/db4c4a7d141fdcaca4d4b11f8fb360db.gif", new Uri("db4c4a7d141fdcaca4d4b11f8fb360db.png", UriKind.Relative)},
{"http://statics.plurk.com/ced6d40bebe2d424b59322b311fc04bb.gif", new Uri("ced6d40bebe2d424b59322b311fc04bb.png", UriKind.Relative)},
{"http://statics.plurk.com/b62d1e55e8311af5bc7526c595ac1dbb.gif", new Uri("b62d1e55e8311af5bc7526c595ac1dbb.png", UriKind.Relative)},
{"http://statics.plurk.com/9b6f4864c822e1a97c98507c2b41a74f.gif", new Uri("9b6f4864c822e1a97c98507c2b41a74f.png", UriKind.Relative)},
{"http://statics.plurk.com/e49c8ae965452550c98fc7f99741ae0d.gif", new Uri("e49c8ae965452550c98fc7f99741ae0d.png", UriKind.Relative)},
{"http://statics.plurk.com/318416eab5a856bddb1e106a21ff557a.gif", new Uri("318416eab5a856bddb1e106a21ff557a.png", UriKind.Relative)},
}
;
        }
    }
}
