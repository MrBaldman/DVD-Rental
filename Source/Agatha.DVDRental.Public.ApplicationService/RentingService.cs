﻿using System;
using System.Collections.Generic;
using System.Linq;
using Agatha.DVDRental.Catalogue.Catalogue;
using Agatha.DVDRental.Catalogue.Infrastructure;
using Agatha.DVDRental.Catalogue.Infrastructure.Indexes;
using Agatha.DVDRental.Domain;
using Agatha.DVDRental.Public.ApplicationService.ApplicationViews;
using Agatha.DVDRental.Public.ApplicationService.Queries;
using Agatha.DVDRental.Subscription.Contracts;
using Agatha.DVDRental.Subscription.Infrastructure;
using Agatha.DVDRental.Subscription.Model.RentalRequests;
using Agatha.DVDRental.Subscription.Model.Subscriptions;
using AutoMapper;
using NServiceBus;
using Raven.Client;

namespace Agatha.DVDRental.Public.ApplicationService
{
    public class RentingService
    {
        private readonly IDocumentSession _ravenDbSession;
        private readonly IBus _bus;
        private readonly RentalRequestRepository _rentalRequestRepository;
        private readonly FilmRepository _filmRepository;
        private ISubscriptionRepository _subscriptionRepository;

        public RentingService(IDocumentSession ravenDbSession, IBus bus, 
                              RentalRequestRepository rentalRequestRepository, FilmRepository filmRepository, 
                              ISubscriptionRepository subscriptionRepository)
        {
            _ravenDbSession = ravenDbSession;
            _bus = bus;
            _rentalRequestRepository = rentalRequestRepository;
            _filmRepository = filmRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        // Methods are like use cases of the system

        public IEnumerable<FilmView> CustomerWantsToViewFilmsAvailableForRent(string memberEmail)
        {

            IEnumerable<FilmResult> query = _ravenDbSession
                .Query<Film, FilmsUnTyped>()
                .Take(100)
                .AsProjection<FilmResult>();

            foreach(FilmResult fr in query)
            {
                Console.WriteLine(fr.Title);
            }


            var subscription = _subscriptionRepository.FindBy(memberEmail);

            // Find all films
            var all_films = _ravenDbSession.Query<Film>().Take(10).ToList();

            var all_filmviews = Mapper.Map<IEnumerable<Film>, IEnumerable<FilmView>>(all_films);
         
            // Find all films in rental list
            RentalRequestList rentalList = GetRentalListFor(subscription.Id);

            if (rentalList != null)
                foreach (var rentalRequest in rentalList.RentalRequests)
                {
                    var film = all_filmviews.SingleOrDefault(x => x.Id == rentalRequest.FilmId);

                    film.IsOnRentalList = true;
                }
            
            return all_filmviews;
        }

        public IEnumerable<RentalRequestView> ViewRentalListFor(string memberEmail)
        {
            var subscription = _subscriptionRepository.FindBy(memberEmail);

            IEnumerable<RentalRequestView> allRequestViews = _ravenDbSession
                                                        .Query<RentalRequestView, RentalRequestIndex>()
                                                        .Take(100)
                                                        .Where(x => x.SubscriptionId == subscription.Id)
                                                        .AsProjection<RentalRequestView>();            
            return allRequestViews.ToList();
        }

        
        public RentalRequestList GetRentalListFor(int subscriptionId)
        {
            RentalRequestList rentalRequestList = _rentalRequestRepository.FindBy(subscriptionId);

            if (rentalRequestList == null)
            {
                rentalRequestList = new RentalRequestList(subscriptionId);
                _rentalRequestRepository.Add(rentalRequestList);
            }

            return rentalRequestList;
        }

    }
}
