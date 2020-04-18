using Amazon.DynamoDBv2.Model;
using RandomUserGenerator.DataAccess.DAO;
using RandomUserGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RandomUserGenerator.Utils
{
    public static class Extensions
    {
        /// <summary>
        /// Map user DAO to usermodel
        /// </summary>        
        /// <param name="imageType">Specify image size to return - default: Thumbnail</param>        
        public static UserModel ToUserModel(this User user, ImageType imageType = ImageType.Thumbnail)
        {
            return new UserModel
            {
                Id = user.id,
                Email = user.email,
                Title = user.title,
                FirstName = user.firstName,
                LastName = user.lastName,
                DateOfBirth = DateTime.Parse(user.dateOfBirth ?? DateTime.UtcNow.ToString()),
                ImageUrl = imageType switch {
                    ImageType.Large => user.largeUrl,
                    ImageType.Medium => user.mediumUrl,
                    _ => user.thumbnailUrl 
                },
                PhoneNumber = user.phone,
                ImageType = imageType
            };
        }

        /// <summary>
        /// Map user model to DAO
        /// </summary>        
        /// <param name="userDAO">optional, pass existing or it will create a new one</param>
        /// <returns></returns>
        public static User ToUserDAO(this UserModel userModel, User userDAO = null)
        {
            userDAO = userDAO ?? new User { id = userModel.Id };
            userDAO.firstName = userModel.FirstName ?? userDAO.firstName;
            userDAO.lastName = userModel.LastName ?? userDAO.lastName;
            userDAO.dateOfBirth = userModel.DateOfBirth?.ToString("o") ?? userDAO.dateOfBirth;
            userDAO.email = userModel.Email ?? userDAO.email;
            userDAO.title = userModel.Title ?? userDAO.title;
            userDAO.phone = userModel.PhoneNumber ?? userDAO.phone;
            switch (userModel.ImageType)
            {
                case ImageType.Thumbnail:
                    userDAO.thumbnailUrl = userModel.ImageUrl ?? userDAO.thumbnailUrl;
                    break;
                case ImageType.Medium:
                    userDAO.mediumUrl = userModel.ImageUrl ?? userDAO.mediumUrl;
                    break;
                case ImageType.Large:
                    userDAO.largeUrl = userModel.ImageUrl ?? userDAO.largeUrl;
                    break;
            }
            return userDAO;
        }

        /// <summary>
        /// Map amazon GetItemResponses to simple objects - only works for numbers and strings, not suitable for arrays or nested objects. Requires case-sensitive string match of property names.
        /// </summary>
        /// <typeparam name="T">Type of object to map to</typeparam>        
        /// <returns></returns>
        public static T MapSimpleResponse<T>(this Dictionary<string, AttributeValue> attributeValues) where T : new()
        {
            var obj = new T();
            foreach (var value in attributeValues)
            {
                try
                {
                    var property = typeof(T).GetProperty(value.Key);
                    switch (property.PropertyType.Name)
                    {
                        case "Int32":
                            property.SetValue(obj, int.Parse(value.Value.N), null);
                            break;
                        default:
                            property.SetValue(obj, value.Value.S, null);
                            break;
                    }
                }
                catch { continue; }
            }
            return obj;
        }

        /// <summary>
        /// Map User DAO to dyanmo request format.
        /// </summary>        
        public static Dictionary<string, AttributeValue> ToDyanmoRequest<T>(this T dao) where T : DynamoDAO
        {
            var dict = new Dictionary<string, AttributeValue>();
            var properties = typeof(T).GetProperties();

            var getAttributeValue = new Func<PropertyInfo, KeyValuePair<string, AttributeValue>>(p => p.PropertyType.Name switch
            {
                "Int32" => new KeyValuePair<string, AttributeValue> (p.Name, new AttributeValue { N = p.GetValue(dao)?.ToString() }),
                _ => new KeyValuePair<string, AttributeValue> (p.Name, new AttributeValue { S = p.GetValue(dao)?.ToString() ?? "Unknown"})
            });

            return properties
                .Select(x => getAttributeValue(x))
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public static User OverwriteUser(this User oldUser, User newUser)
        {
            bool isCreateUser = oldUser.id == 0;

            var properties = typeof(User).GetProperties();            
            var returnUser = oldUser;

            var doOverwrite = new Action<PropertyInfo>(p => {
                if (p.Name != "id")
                {
                    var oldValue = p.GetValue(oldUser);
                    var newValue = p.GetValue(newUser);
                    if (oldValue != (newValue ?? oldValue))
                        p.SetValue(returnUser, p.GetValue(newUser));
                }
                else if (isCreateUser)
                    returnUser.id = newUser.id;
            });

            foreach (var property in properties)            
                doOverwrite(property);            

            return returnUser;
        }
    }
}
