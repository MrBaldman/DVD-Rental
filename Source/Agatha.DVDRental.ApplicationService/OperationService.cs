﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agatha.DVDRental.ApplicationService
{
    public class OperationService
    {
        // Methods are like use cases of the system

        public void OperatorWantsToAddStock(int filmId, string barcode  )
        {

        }       

        public void OperatorWantsToProceesAFilmReturn(string barcode)
        {

        }

        public void OperatorWantsToPickRentalAllocations(string processorName)
        {

        }

        public string OperatorWantsToViewAssignedRentalAllocations(string processorName)
        {
            return "";
        }

        public void OperatorWantsToMarkRentalAllocationsAsDispatched(string processorName)
        {

        }
    }
}
