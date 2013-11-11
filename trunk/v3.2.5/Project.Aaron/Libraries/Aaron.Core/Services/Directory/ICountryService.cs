using System.Collections.Generic;
using Aaron.Core.Domain.Directory;

namespace Aaron.Core.Services.Directory
{
    /// <summary>
    /// Country service interface
    /// </summary>
    public partial interface ICountryService : IServices
    {
        /// <summary>
        /// Deletes a country
        /// </summary>
        /// <param name="country">Country</param>
        void DeleteCountry(Country country);

        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Country collection</returns>
        IList<Country> GetAllCountries(bool showHidden = false);

        /// <summary>
        /// Gets a country 
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Country</returns>
        Country GetCountryById(int countryId);

        /// <summary>
        /// Gets a country by two letter ISO code
        /// </summary>
        /// <param name="twoLetterIsoCode">Country two letter ISO code</param>
        /// <returns>Country</returns>
        Country GetCountryByTwoLetterIsoCode(string twoLetterIsoCode);

        /// <summary>
        /// Gets a country by three letter ISO code
        /// </summary>
        /// <param name="threeLetterIsoCode">Country three letter ISO code</param>
        /// <returns>Country</returns>
        Country GetCountryByThreeLetterIsoCode(string threeLetterIsoCode);

        /// <summary>
        /// Inserts a country
        /// </summary>
        /// <param name="country">Country</param>
        void InsertCountry(Country country);

        /// <summary>
        /// Updates the country
        /// </summary>
        /// <param name="country">Country</param>
        void UpdateCountry(Country country);
    }
}