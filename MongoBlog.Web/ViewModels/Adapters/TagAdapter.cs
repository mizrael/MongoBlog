using System.Linq;
using System.Web;
using MongoBlog.Core;
using MongoBlog.Models;

namespace MongoBlog.Web.ViewModels.Adapters
{
    public static class TagAdapter
    {
        public static TagVM Adapt(HttpContextBase context, Tag m)
        {
            var vm = new TagVM() { PostsCount = m.PostsCount, Slug = m.Slug };
            if (null != m.Translations)
            {
                var currLang = LanguageProvider.GetUICurrentLanguageCode(context);

                var translation = m.Translations.FirstOrDefault(t => t.LanguageCode == currLang) ??
                                  m.Translations.FirstOrDefault();

                if (null != translation)
                    vm.Text = translation.Text;
            }
            return vm;
        }
    }
}