using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Models
{
    /// <summary>
    /// Mapped property model for <see cref="PropertyCategory"/>
    /// </summary>
    public class CategoryPropertyModel : PropertyModel<IEnumerable<CategoryModel>, PropertyCategory>
    {
        private Injected<CategoryRepository> _categoryRepository = new Injected<CategoryRepository>();

        /// <summary>
        /// We only need CategoryList from PropertyCategory.
        /// Basicaly, this constructor loop through CategoryList of PropertyCategory, retrieve each category from categoryRepository, convert it to view model
        /// and add to Value prop
        /// </summary>
        /// <param name="propertyCategory"></param>
        public CategoryPropertyModel(PropertyCategory propertyCategory) : base(propertyCategory)
        {
            var categoryModels = new List<CategoryModel>();
            if (propertyCategory.Category != null)
            {
                foreach (var id in propertyCategory.Category)
                {
                    var category = _categoryRepository.Service.Get(id);
                    categoryModels.Add(new CategoryModel()
                    {
                        Id = category.ID,
                        Name = category.Name,
                        Description = category.Description
                    });
                }
            }

            Value = categoryModels;
        }
    }
}