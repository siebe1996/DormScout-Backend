using DataAccessLayer.Repositories.interfaces;
using Globals.Entities;
using Globals.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Models.Coordinates;
using Models.Notes;
using Models.PlaceImages;
using Models.Places;
using Models.PossibleDates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace DataAccessLayer.Repositories
{
    public class PlaceRepository : IPlaceRepository
    {
        private readonly Backend_DormScoutContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;

        public PlaceRepository(Backend_DormScoutContext backend_DormScoutContext, IHttpContextAccessor httpContextAccessor)
        {
            _context = backend_DormScoutContext;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
        }

        public async Task<List<GetPlaceModel>> GetPlacesReviewer()
        {
            Guid userId = new Guid(_user.Identity.Name);
            List<GetPlaceModel> places = await _context.Places
                .AsNoTracking()
                .Where(x => x.ReviewerId == userId)
                .Select(x => new GetPlaceModel
                {
                    Id = x.Id,
                    Address = x.Address,
                    ChosenDate = x.ChosenDate,
                    RequesterId = x.RequesterId,
                    ReviewerId = x.ReviewerId,
                    ReviewId = x.ReviewId,
                    HomeownerTelephone = x.HomeownerTelephone,
                    HomeownerEmail = x.HomeownerEmail,
                    Link = x.Link,
                    Dates = x.Dates.Select(x => new GetPossibleDateModel
                    {
                        Id = x.Id,
                        PlaceId = x.PlaceId,
                        Date = x.Date,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    }).ToList(),
                    Notes = x.Notes.Select(x => new GetNoteModel
                    {
                        Id = x.Id,
                        PlaceId = x.PlaceId,
                        Content = x.Content,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    }).ToList(),
                    Images = x.Images.Select(x => new GetPlaceImageModel
                    {
                        Id = x.Id,
                        PlaceId = x.PlaceId,
                        ImageData = x.ImageData,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    }).ToList(),
                    Coordinate = new GetCoordinateModel
                    {
                        Id = x.Coordinate.Id,
                        Latitude = x.Coordinate.Latitude,
                        Longitude = x.Coordinate.Longitude,
                        CreatedAt = x.Coordinate.CreatedAt,
                        UpdatedAt = x.Coordinate.UpdatedAt,
                    },
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                })
            .ToListAsync();

            return places;
        }

        public async Task<List<GetPlaceModel>> GetPlacesNotYours()
        {
            //toDo make sure the dates havnt passed utc.now
            Guid userId = new Guid(_user.Identity.Name);
            List<GetPlaceModel> places = await _context.Places
                .AsNoTracking()
                .Where(x => x.RequesterId !=  userId && x.ReviewerId == null)
                .Select(x => new GetPlaceModel
            {
                Id = x.Id,
                Address = x.Address,
                ChosenDate = x.ChosenDate,
                RequesterId = x.RequesterId,
                ReviewerId = x.ReviewerId,
                    ReviewId = x.ReviewId,
                    HomeownerTelephone = x.HomeownerTelephone,
                HomeownerEmail = x.HomeownerEmail,
                Link = x.Link,
                Dates = x.Dates.Select(x => new GetPossibleDateModel
                {
                     Id = x.Id,
                     PlaceId = x.PlaceId,
                     Date = x.Date,
                     CreatedAt = x.CreatedAt,
                     UpdatedAt = x.UpdatedAt,
                }).ToList(),
                Notes = x.Notes.Select(x => new GetNoteModel{
                    Id = x.Id,
                    PlaceId = x.PlaceId,
                    Content = x.Content,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    }).ToList(),
                    Images = x.Images.Select(x => new GetPlaceImageModel
                    {
                        Id = x.Id,
                        PlaceId = x.PlaceId,
                        ImageData = x.ImageData,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    }).ToList(),
                    Coordinate = new GetCoordinateModel
                    {
                        Id = x.Coordinate.Id,
                        Latitude = x.Coordinate.Latitude,
                        Longitude = x.Coordinate.Longitude,
                        CreatedAt = x.Coordinate.CreatedAt,
                        UpdatedAt = x.Coordinate.UpdatedAt,
                    },
                    CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            })
            .ToListAsync();

            return places;
        }

        public async Task<List<GetPlaceModel>> GetPlacesYours()
        {
            Guid userId = new Guid(_user.Identity.Name);
            List<GetPlaceModel> places = await _context.Places
                .AsNoTracking()
                .Where(x => x.RequesterId == userId)
                .Select(x => new GetPlaceModel
                {
                    Id = x.Id,
                    Address = x.Address,
                    ChosenDate = x.ChosenDate,
                    RequesterId = x.RequesterId,
                    ReviewerId = x.ReviewerId,
                    ReviewId = x.ReviewId,
                    HomeownerTelephone = x.HomeownerTelephone,
                    HomeownerEmail = x.HomeownerEmail,
                    Link = x.Link,
                    Dates = x.Dates.Select(x => new GetPossibleDateModel
                    {
                        Id = x.Id,
                        PlaceId = x.PlaceId,
                        Date = x.Date,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    }).ToList(),
                    Notes = x.Notes.Select(x => new GetNoteModel
                    {
                        Id = x.Id,
                        PlaceId = x.PlaceId,
                        Content = x.Content,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    }).ToList(),
                    Images = x.Images.Select(x => new GetPlaceImageModel
                    {
                        Id = x.Id,
                        PlaceId = x.PlaceId,
                        ImageData = x.ImageData,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    }).ToList(),
                    Coordinate = new GetCoordinateModel
                    {
                        Id = x.Coordinate.Id,
                        Latitude = x.Coordinate.Latitude,
                        Longitude = x.Coordinate.Longitude,
                        CreatedAt = x.Coordinate.CreatedAt,
                        UpdatedAt = x.Coordinate.UpdatedAt,
                    },
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                })
            .ToListAsync();

            return places;
        }

        public async Task<List<GetPlaceModel>> GetPlacesNearby(double lat, double lon)
        {
            //toDo make sure the dates havnt passed utc.now
            Guid userId = new Guid(_user.Identity.Name);
            double maxRadiusKm = 10;
            var placesWithoutDistance = await _context.Places
                .AsNoTracking()
                .Where(x => x.RequesterId != userId && x.ReviewerId == null)
                .Select(x => new
                {
                    Place = x,
                    Coordinate = x.Coordinate,
                })
                .ToListAsync();

            List<GetPlaceModel> places = placesWithoutDistance
                .Where(x => GetDistanceFromLatLonInKm(lat, lon, x.Coordinate.Latitude, x.Coordinate.Longitude) <= maxRadiusKm)
                .Select(x => new GetPlaceModel
                {
                    Id = x.Place.Id,
                    Address = x.Place.Address,
                    ChosenDate = x.Place.ChosenDate,
                    RequesterId = x.Place.RequesterId,
                    ReviewerId = x.Place.ReviewerId,
                    ReviewId = x.Place.ReviewId,
                    HomeownerTelephone = x.Place.HomeownerTelephone,
                    HomeownerEmail = x.Place.HomeownerEmail,
                    Link = x.Place.Link,
                    Dates = x.Place.Dates.Select(x => new GetPossibleDateModel
                    {
                        Id = x.Id,
                        PlaceId = x.PlaceId,
                        Date = x.Date,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    }).ToList(),
                    Notes = x.Place.Notes.Select(x => new GetNoteModel
                    {
                        Id = x.Id,
                        PlaceId = x.PlaceId,
                        Content = x.Content,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    }).ToList(),
                    Images = x.Place.Images.Select(x => new GetPlaceImageModel
                    {
                        Id = x.Id,
                        PlaceId = x.PlaceId,
                        ImageData = x.ImageData,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    }).ToList(),
                    Coordinate = new GetCoordinateModel
                    {
                        Id = x.Coordinate.Id,
                        Latitude = x.Coordinate.Latitude,
                        Longitude = x.Coordinate.Longitude,
                        CreatedAt = x.Coordinate.CreatedAt,
                        UpdatedAt = x.Coordinate.UpdatedAt,
                    },
                    CreatedAt = x.Place.CreatedAt,
                    UpdatedAt = x.Place.UpdatedAt,
                })
                .ToList();

            /*List<GetPlaceModel> places = await _context.Places
                .AsNoTracking()
                .Where(x => x.RequesterId != userId && x.ReviewerId == null)
                .Where(x => GetDistanceFromLatLonInKm(lat, lon, x.Coordinate.Latitude, x.Coordinate.Longitude) <= maxRadiusKm)
                .Select(x => new GetPlaceModel
                {
                    Id = x.Id,
                    Address = x.Address,
                    ChosenDate = x.ChosenDate,
                    RequesterId = x.RequesterId,
                    ReviewerId = x.ReviewerId,
                    ReviewId = x.ReviewId,
                    HomeownerTelephone = x.HomeownerTelephone,
                    HomeownerEmail = x.HomeownerEmail,
                    Link = x.Link,
                    Dates = x.Dates.Select(x => new GetPossibleDateModel
                    {
                        Id = x.Id,
                        PlaceId = x.PlaceId,
                        Date = x.Date,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    }).ToList(),
                    Notes = x.Notes.Select(x => new GetNoteModel
                    {
                        Id = x.Id,
                        PlaceId = x.PlaceId,
                        Content = x.Content,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    }).ToList(),
                    Images = x.Images.Select(x => new GetPlaceImageModel
                    {
                        Id = x.Id,
                        PlaceId = x.PlaceId,
                        ImageData = x.ImageData,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    }).ToList(),
                    Coordinate = new GetCoordinateModel
                    {
                        Id = x.Coordinate.Id,
                        Latitude = x.Coordinate.Latitude,
                        Longitude = x.Coordinate.Longitude,
                        CreatedAt = x.Coordinate.CreatedAt,
                        UpdatedAt = x.Coordinate.UpdatedAt,
                    },
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                })
            .ToListAsync();*/

            return places;
        }

        private double GetDistanceFromLatLonInKm (double lat1, double lon1, double lat2, double lon2) {
            const double earthRadiusKm = 6371;

            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = earthRadiusKm * c;

            return distance;
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        public async Task<GetPlaceModel> GetPlace(Guid id)
        {
            GetPlaceModel place = await _context.Places
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new GetPlaceModel
                {
                    Id = x.Id,
                    Address = x.Address,
                    ChosenDate = x.ChosenDate,
                    RequesterId = x.RequesterId,
                    ReviewerId = x.ReviewerId,
                    ReviewId = x.ReviewerId,
                    HomeownerTelephone = x.HomeownerTelephone,
                    HomeownerEmail = x.HomeownerEmail,
                    Link = x.Link,
                    Dates = x.Dates.Select(x => new GetPossibleDateModel
                    {
                        Id = x.Id,
                        PlaceId = x.PlaceId,
                        Date = x.Date,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    }).ToList(),
                    Notes = x.Notes.Select(x => new GetNoteModel
                    {
                        Id = x.Id,
                        PlaceId = x.PlaceId,
                        Content = x.Content,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    }).ToList(),
                    Images = x.Images.Select(x => new GetPlaceImageModel
                    {
                        Id = x.Id,
                        PlaceId = x.PlaceId,
                        ImageData = x.ImageData,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    }).ToList(),
                    Coordinate = new GetCoordinateModel
                    {
                        Id = x.Coordinate.Id,
                        Latitude = x.Coordinate.Latitude,
                        Longitude = x.Coordinate.Longitude,
                        CreatedAt = x.Coordinate.CreatedAt,
                        UpdatedAt = x.Coordinate.UpdatedAt,
                    },
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                })
                .FirstOrDefaultAsync();
            if (place == null)
            {
                throw new NotFoundException("Place Not Found");
            }
            return place;
        }

        public async Task<GetPlaceModel> PostPlace(PostPlaceModel postPlaceModel)
        {
            Guid userId = new Guid(_user.Identity.Name);
            Place place = new Place
            {
                Address = postPlaceModel.Address,
                RequesterId = userId,
                HomeownerTelephone = postPlaceModel.HomeownerTelephone,
                HomeownerEmail = postPlaceModel.HomeownerEmail,
                Link = postPlaceModel.Link,
                Dates = postPlaceModel.Dates?.Select(x => new PossibleDate
                {
                    Date = x.Date,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                }).ToList(),
                Notes = postPlaceModel.Notes?.Select(x => new Note
                {
                    Content = x.Content,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                }).ToList(),
                Images = postPlaceModel.Images?.Select(x => new PlaceImage
                {
                    ImageData = x.ImageData,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                }).ToList(),
                Coordinate = new Coordinate
                {
                    Latitude = postPlaceModel.Coordinate.Latitude,
                    Longitude = postPlaceModel.Coordinate.Longitude,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _context.Places.Add(place);
            await _context.SaveChangesAsync();

            GetPlaceModel getPlaceModel = new GetPlaceModel
            {
                Id = place.Id,
                Address = place.Address,
                RequesterId = place.RequesterId,
                HomeownerTelephone = place.HomeownerTelephone,
                HomeownerEmail = place.HomeownerEmail,
                Link = place.Link,
                Dates = place.Dates.Select(x => new GetPossibleDateModel
                {
                    Id = x.Id,
                    PlaceId = x.PlaceId,
                    Date = x.Date,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).ToList(),
                Notes = place.Notes.Select(x => new GetNoteModel
                {
                    Id = x.Id,
                    PlaceId = x.PlaceId,
                    Content = x.Content,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).ToList(),
                Images = place.Images.Select(x => new GetPlaceImageModel
                {
                    Id = x.Id,
                    PlaceId = x.PlaceId,
                    ImageData = x.ImageData,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).ToList(),
                Coordinate = new GetCoordinateModel 
                { 
                    Id = place.Coordinate.Id,
                    Latitude = place.Coordinate.Latitude,
                    Longitude = place.Coordinate.Longitude,
                    CreatedAt = place.Coordinate.CreatedAt,
                    UpdatedAt = place.Coordinate.UpdatedAt,
                },
                CreatedAt = place.CreatedAt,
                UpdatedAt = place.UpdatedAt,
            };
            return getPlaceModel;
        }

        public async Task<GetPlaceModel> PutPlace (Guid id, PutPlaceModel putPlaceModel)
        { 
            Place place = await _context.Places.FirstOrDefaultAsync(p => p.Id == id);
            if ( place == null)
            {
                throw new NotFoundException("Place not found");
            }

            if ( putPlaceModel.ChosenDate != null)
            {
                place.ChosenDate = putPlaceModel.ChosenDate;
            }
            else
            {
                place.ChosenDate = null;
            }

            if (putPlaceModel.ReviewerId != null)
            {
                place.ReviewerId = putPlaceModel.ReviewerId;
            }
            else
            {
                place.ReviewerId = null;
            }

            if (putPlaceModel.ReviewId != null && putPlaceModel.ReviewId != Guid.Empty)
            {
                place.ReviewId = putPlaceModel.ReviewId;
            }

            place.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            GetPlaceModel getPlaceModel = new GetPlaceModel
            {
                Id = place.Id,
                Address = place.Address,
                RequesterId = place.RequesterId,
                HomeownerTelephone = place.HomeownerTelephone,
                HomeownerEmail = place.HomeownerEmail,
                Link = place.Link,
                Dates = place.Dates.Select(x => new GetPossibleDateModel
                {
                    Id = x.Id,
                    PlaceId = x.PlaceId,
                    Date = x.Date,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).ToList(),
                Notes = place.Notes.Select(x => new GetNoteModel
                {
                    Id = x.Id,
                    PlaceId = x.PlaceId,
                    Content = x.Content,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).ToList(),
                Images = place.Images.Select(x => new GetPlaceImageModel
                {
                    Id = x.Id,
                    PlaceId = x.PlaceId,
                    ImageData = x.ImageData,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).ToList(),
                Coordinate = new GetCoordinateModel
                {
                    Id = place.Coordinate.Id,
                    Latitude = place.Coordinate.Latitude,
                    Longitude = place.Coordinate.Longitude,
                    CreatedAt = place.Coordinate.CreatedAt,
                    UpdatedAt = place.Coordinate.UpdatedAt,
                },
                CreatedAt = place.CreatedAt,
                UpdatedAt = place.UpdatedAt,
            };
            return getPlaceModel;
        }

        public async Task<GetPlaceModel> PatchPlace(Guid id, PatchPlaceModel patchPlaceModel)
        {
            Place place = await _context.Places.FirstOrDefaultAsync(p => p.Id == id);

            if (place == null)
            {
                throw new NotFoundException("Place not found");
            }

            // Update properties based on the PatchPlaceModel
            if (patchPlaceModel.Address != null)
            {
                place.Address = patchPlaceModel.Address;
            }

            if (patchPlaceModel.HomeownerTelephone != null)
            {
                place.HomeownerTelephone = patchPlaceModel.HomeownerTelephone;
            }

            if (patchPlaceModel.HomeownerEmail != null)
            {
                place.HomeownerEmail = patchPlaceModel.HomeownerEmail;
            }

            if (patchPlaceModel.Link != null)
            {
                place.Link = patchPlaceModel.Link;
            }

            if (patchPlaceModel.Coordinate != null)
            {
                place.Coordinate.Latitude = patchPlaceModel.Coordinate.Latitude;
                place.Coordinate.Longitude = patchPlaceModel.Coordinate.Longitude;
                place.Coordinate.UpdatedAt = DateTime.UtcNow;
            }

            var oldDates = place.Dates.ToList();

            if (patchPlaceModel.Dates != null && patchPlaceModel.Dates.Any())
            {
                foreach (var date in patchPlaceModel.Dates)
                {
                    var existingDate = place.Dates.FirstOrDefault(d => d.Id == date.Id);
                    if (existingDate == null)
                    {
                        place.Dates.Add(new PossibleDate
                        {
                            Date = date.Date,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                        });
                    }
                    else if (existingDate != null && existingDate.Date != date.Date)
                    {
                        existingDate.Date = date.Date;
                        existingDate.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }

            foreach (var oldDate in oldDates)
            {
                if (!patchPlaceModel.Dates.Any(d => d.Id == oldDate.Id))
                {
                    place.Dates.Remove(oldDate);
                }
            }

            var oldNotes = place.Notes.ToList();

            if (patchPlaceModel.Notes != null && patchPlaceModel.Notes.Any())
            {
                foreach(var note in patchPlaceModel.Notes)
                {
                    var existingNote = place.Notes.FirstOrDefault(n => n.Id == note.Id);
                    if (existingNote == null)
                    {
                        place.Notes.Add(new Note { 
                            Content = note.Content,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                        });
                    }
                    else if (existingNote != null && existingNote.Content != note.Content)
                    {
                        existingNote.Content = note.Content;
                        existingNote.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }

            foreach ( var oldNote  in oldNotes)
            {
                if(!patchPlaceModel.Notes.Any(n => n.Id == oldNote.Id))
                {
                    place.Notes.Remove(oldNote);
                }
            }

            var oldImages = place.Images.ToList();

            if (patchPlaceModel.Images != null && patchPlaceModel.Images.Any())
            {
                foreach (var image in patchPlaceModel.Images)
                {
                    var existingImage = place.Images.FirstOrDefault(n => n.Id == image.Id);
                    if (existingImage == null)
                    {
                        place.Images.Add(new PlaceImage
                        {
                            ImageData = image.ImageData,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                        });
                    }
                    else if (existingImage != null && existingImage.ImageData != image.ImageData) 
                    {
                        existingImage.ImageData = image.ImageData;
                        existingImage.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }
            foreach (var oldImage in oldImages)
            {
                if (!patchPlaceModel.Images.Any(i => i.Id == oldImage.Id))
                {
                    place.Images.Remove(oldImage);
                }
            }

            await _context.SaveChangesAsync();

            GetPlaceModel getPlaceModel = new GetPlaceModel
            {
                Id = place.Id,
                Address = place.Address,
                RequesterId = place.RequesterId,
                HomeownerTelephone = place.HomeownerTelephone,
                HomeownerEmail = place.HomeownerEmail,
                Link = place.Link,
                Dates = place.Dates.Select(x => new GetPossibleDateModel
                {
                    Id = x.Id,
                    PlaceId = x.PlaceId,
                    Date = x.Date,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).ToList(),
                Notes = place.Notes.Select(x => new GetNoteModel
                {
                    Id = x.Id,
                    PlaceId = x.PlaceId,
                    Content = x.Content,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).ToList(),
                Images = place.Images.Select(x => new GetPlaceImageModel
                {
                    Id = x.Id,
                    PlaceId = x.PlaceId,
                    ImageData = x.ImageData,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).ToList(),
                Coordinate = new GetCoordinateModel
                {
                    Id = place.Coordinate.Id,
                    Latitude = place.Coordinate.Latitude,
                    Longitude = place.Coordinate.Longitude,
                    CreatedAt = place.Coordinate.CreatedAt,
                    UpdatedAt = place.Coordinate.UpdatedAt,
                },
                CreatedAt = place.CreatedAt,
                UpdatedAt = place.UpdatedAt,
            };

            return getPlaceModel;
        }

    }
}
