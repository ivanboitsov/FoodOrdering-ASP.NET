using FoodOrderingWebsite.Data;
using FoodOrderingWebsite.Models;
using FoodOrderingWebsite.Models.Address;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text.RegularExpressions;
using static FoodOrderingWebsite.Services.DishService;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FoodOrderingWebsite.Services
{
    public class AddressService
    {
        private readonly ApplicationDbContext _context;
        public AddressService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Dictionary<string, string> typeMappings = new Dictionary<string, string>
        {
            { "ул", "Элемент улично-дорожной сети" },
            { "пос", "Населенный пункт" },
            { "с", "Населенный пункт" },
            { "тер", "Населенный пункт" },
            { "г", "Населенный пункт" },
            { "р-н", "Административный район" },
            { "обл", "Субъект РФ" }
        };

        public Guid getObjGuidById(long id)
        {
            HouseModel house = _context.as_houses.FirstOrDefault(h => h.objectid == id);
            AddressObjectModel address = _context.as_addr_obj.FirstOrDefault(h => h.objectid == id);
            return house != null ? house.objectguid : address.objectguid;
        }

        public bool checkObjByGuid(Guid guid)
        {
            HouseModel house = _context.as_houses.FirstOrDefault(h => h.objectguid == guid);
            AddressObjectModel address = _context.as_addr_obj.FirstOrDefault(h => h.objectguid == guid);
            if (house != null || address != null)
            {
                return true;
            }
            return false;
        }

        public string getObjName(long id)
        {
            HouseModel house = _context.as_houses.FirstOrDefault(h => h.objectid == id);
            AddressObjectModel address = _context.as_addr_obj.FirstOrDefault(h => h.objectid == id);
            return house != null ? house.housenum : address.name;
        }

        public string getObjLevel(long id)
        {
            HouseModel house = _context.as_houses.FirstOrDefault(h => h.objectid == id);
            AddressObjectModel address = _context.as_addr_obj.FirstOrDefault(h => h.objectid == id);
            return house != null ? "Building" : address.typename;
        }

        public string getObjLevelText(long id)
        {
            HouseModel house = _context.as_houses.FirstOrDefault(h => h.objectid == id);
            AddressObjectModel address = _context.as_addr_obj.FirstOrDefault(h => h.objectid == id);
            return house != null ? "Здание (Строение)" : typeMappings[address.typename];
        }

        public bool checkQuery(string name, string? query)
        {
            return query == null ? true : name.ToUpper().Contains(query.ToUpper());
        }

        public List<AddressResponseModel>? SearchAddress(int parent, string? query)
        {
            if (query != null)
            {
                query = query.ToUpper();
            }

            List<HierarchyModel> sons = _context.as_adm_hierarchy.Where(item => item.parentobjid == parent).ToList();
            List<AddressResponseModel> addresses = new List<AddressResponseModel>();
            foreach (HierarchyModel son in sons)
            {
                AddressObjectModel address = _context.as_addr_obj.FirstOrDefault(addr => addr.objectid == son.objectid);
                if (address != null)
                {
                    if (checkQuery(address.name, query) || checkQuery(address.typename, query))
                    {
                        addresses.Add(new AddressResponseModel
                        {
                            Id = address.id,
                            objectGuid = address.objectguid,
                            text = getObjName(address.objectid),
                            objectLevel = getObjLevel(address.objectid),
                            objectLevelText = getObjLevelText(address.objectid)
                        });
                    }
                }
                HouseModel house = _context.as_houses.FirstOrDefault(h => h.objectid == son.objectid);
                if (house != null)
                {
                    if (checkQuery(house.housenum, query))
                    {
                        addresses.Add(new AddressResponseModel
                        {
                            Id = house.id,
                            objectGuid = house.objectguid,
                            text = getObjName(house.objectid),
                            objectLevel = getObjLevel(house.objectid),
                            objectLevelText = getObjLevelText(house.objectid)
                        });
                    }
                }
            }
            return addresses;
        }

        public List<AddressResponseModel> SearchAddressChain(Guid objectGuid)
        {
            List<AddressResponseModel> addresses = new List<AddressResponseModel>();
            AddressObjectModel rawAddress = _context.as_addr_obj.FirstOrDefault(addr => addr.objectguid == objectGuid);
            HierarchyModel hierarchy = new HierarchyModel();
            if (rawAddress != null)
            {
                hierarchy = _context.as_adm_hierarchy.FirstOrDefault(h => h.objectid == rawAddress.objectid);
            }
            else
            {
                HouseModel rawHouse = _context.as_houses.FirstOrDefault(ho => ho.objectguid == objectGuid);
                hierarchy = _context.as_adm_hierarchy.FirstOrDefault(h => h.objectid == rawHouse.objectid);
            }

            addresses.Add(new AddressResponseModel
            {
                Id = hierarchy.id,
                objectGuid = getObjGuidById(hierarchy.objectid),
                text = getObjName(hierarchy.objectid),
                objectLevel = getObjLevel(hierarchy.objectid),
                objectLevelText = getObjLevelText(hierarchy.objectid)
            });

            while (hierarchy != null)
            {
                long parentid = hierarchy.parentobjid;
                hierarchy = _context.as_adm_hierarchy.FirstOrDefault(h => h.objectid == parentid);
                if (hierarchy != null)
                {
                    addresses.Add(new AddressResponseModel
                    {
                        Id = hierarchy.id,
                        objectGuid = getObjGuidById(hierarchy.objectid),
                        text = getObjName(hierarchy.objectid),
                        objectLevel = getObjLevel(hierarchy.objectid),
                        objectLevelText = getObjLevelText(hierarchy.objectid)
                    });
                }
            }

            addresses.Reverse();

            return addresses;
        }
    }
}
