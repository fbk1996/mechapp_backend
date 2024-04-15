using MechAppBackend.Data;
using MechAppBackend.Helpers;
using MechAppBackend.Models;
using MySql.Data.MySqlClient;

namespace MechAppBackend.features
{
    public class vehicles
    {
        MechAppContext _context;

        public vehicles(MechAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new vehicle or reactivates an existing one in the system based on provided vehicle details.
        /// Validates the required fields and checks for the uniqueness of VIN and registration number combination.
        ///
        /// Parameters:
        /// - _producer: Producer or manufacturer of the vehicle.
        /// - _model: Model of the vehicle.
        /// - _produceDate: Production date of the vehicle.
        /// - _mileage: Mileage of the vehicle in kilometers.
        /// - _vin: Vehicle Identification Number, must be unique.
        /// - _engineNumber: Engine number of the vehicle.
        /// - _registrationNumber: Registration number of the vehicle, must be unique in combination with VIN.
        /// - _enginePower: Power of the engine in horsepower.
        /// - _engineCapacity: Capacity of the engine in cubic centimeters.
        /// - _fuelType: Type of fuel the vehicle uses.
        /// - _owner: ID of the owner of the vehicle.
        ///
        /// Returns:
        /// - "vehicle_added": Successfully added or reactivated the vehicle.
        /// - "exists": The vehicle with provided VIN and registration number already exists and is active.
        /// - Error messages for missing or invalid data such as "no_producer", "no_model", "no_vin", etc.
        /// - "error": General error message for database operation failures.
        /// </summary>
        /// <returns>A string indicating the result of the operation.</returns>
        public string AddVehicle (string _producer, string _model, string _produceDate, int _mileage, string _vin, string _engineNumber,
            string _registrationNumber, int _enginePower, string _engineCapacity, string _fuelType, int _owner)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(_producer))
                return "no_producer";
            if (string.IsNullOrEmpty(_model))
                return "no_model";
            if (string.IsNullOrEmpty(_produceDate))
                return "no_produce_date";
            if (string.IsNullOrEmpty(_vin))
                return "no_vin";
            if (string.IsNullOrEmpty(_registrationNumber))
                return "no_registration_number";
            if (string.IsNullOrEmpty(_fuelType))
                return "no_fuel_type";
            if (_enginePower == null || _enginePower == 0)
                return "no_engine_power";
            // Validate field lengths
            if (!Validators.CheckLength(_producer, 50))
                return "producer_length";
            if (!Validators.CheckLength(_model, 80))
                return "model_length";
            if (!Validators.CheckLength(_vin, 18))
                return "vin_length";

            try
            {
                // Check for existing vehicle
                var exist = _context.UsersVehicles.FirstOrDefault(v => v.Vin == _vin && v.Owner == _owner && v.RegistrationNumber == _registrationNumber);

                if (exist != null)
                {
                    // Reactivate vehicle if deleted
                    if (exist.IsDeleted == 1)
                    {
                        exist.IsDeleted = 0;
                        _context.SaveChanges();
                        return "vehicle_added";
                    }
                    else return "exists";
                }
                // Add new vehicle
                _context.UsersVehicles.Add(new UsersVehicle
                {
                    Producer = _producer,
                    Model = _model,
                    ProduceDate = _produceDate,
                    Mileage = _mileage,
                    Vin = _vin,
                    EngineNumber = _engineNumber,
                    RegistrationNumber = _registrationNumber,
                    EnginePower = _enginePower,
                    EngineCapacity = _engineCapacity,
                    FuelType = _fuelType,
                    Owner = _owner
                });

                _context.SaveChanges();

                return "vehicle_added";
            }
            catch (MySqlException ex)
            {
                Logger.SendException("mechapp", "vehicles", "addVehicle", ex);
                return "error";
            }
        }

        /// <summary>
        /// Edits an existing vehicle in the system.
        /// Updates vehicle details based on the provided parameters. Validates required fields 
        /// and checks if the vehicle exists in the database before updating its details.
        ///
        /// Parameters:
        /// - _id: The database ID of the vehicle to be edited.
        /// - _produceDate: The updated production date of the vehicle.
        /// - _mileage: The updated mileage of the vehicle.
        /// - _engineNumber: The updated engine number of the vehicle.
        /// - _registrationNumber: The updated registration number of the vehicle.
        /// - _enginePower: The updated power of the vehicle's engine (in horsepower).
        /// - _engineCapacity: The updated engine capacity (in cubic centimeters).
        ///
        /// Returns:
        /// - "vehicle_edited": Successfully updated the vehicle's information.
        /// - "no_produce_date": Production date not provided.
        /// - "no_registration_number": Registration number not provided.
        /// - "no_engine_power": Engine power not provided or set to zero, which is invalid.
        /// - "error": General error message for invalid vehicle ID or database operation failures.
        /// </summary>
        /// <returns>A string indicating the result of the operation.</returns>
        public string EditVehicle(int _id, string _produceDate, int _mileage, string _engineNumber,
            string _registrationNumber, int _enginePower, string _engineCapacity)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(_produceDate))
                return "no_produce_date";
            if (string.IsNullOrEmpty(_registrationNumber))
                return "no_registration_number";
            if (_enginePower == null || _enginePower == 0)
                return "no_engine_power";
            // Validate vehicle ID
            if (_id == null || _id == -1)
                return "error";

            try
            {
                // Retrieve the vehicle from the database
                var vehicle = _context.UsersVehicles.FirstOrDefault(v => v.Id == _id);

                if (vehicle == null)
                    return "error";
                // Update vehicle details
                vehicle.ProduceDate = _produceDate;
                vehicle.Mileage = _mileage;
                vehicle.EngineNumber = _engineNumber;
                vehicle.RegistrationNumber = _registrationNumber;
                vehicle.EnginePower = _enginePower;
                vehicle.EngineCapacity = _engineCapacity;
                // Save the changes to the database
                _context.SaveChanges();

                return "vehicle_edited";
            }
            catch (MySqlException ex)
            {
                // Handle exceptions and return an error message
                Logger.SendException("MechApp", "vehicles", "EditVehicle", ex);
                return "error";
            }
        }

        /// <summary>
        /// Soft Deletes a vehicle in the system.
        /// Marks a vehicle as deleted in the database based on its ID instead of physically removing it. 
        /// This allows for data recovery and historical integrity. Validates the vehicle ID
        /// and checks if the vehicle exists before marking it as deleted.
        ///
        /// Parameters:
        /// - _id: The database ID of the vehicle to be soft deleted.
        ///
        /// Returns:
        /// - "vehicle_deleted": Indicates that the vehicle was successfully marked as deleted.
        /// - "error": Returns an error if the vehicle ID is invalid, the vehicle does not exist, 
        ///   or there is a database operation failure.
        /// </summary>
        /// <param name="_id">The ID of the vehicle to delete.</param>
        /// <returns>A string indicating the result of the operation: either "vehicle_deleted" or "error".</returns>
        public string DeleteVehicle(int _id)
        {
            // Validate the vehicle ID
            if (_id == null || _id == -1)
                return "error";

            try
            {
                // Retrieve the vehicle from the database
                var vehicle = _context.UsersVehicles.FirstOrDefault(v => v.Id == _id);
                if (vehicle == null)
                    return "error";

                // Mark the vehicle as deleted instead of removing it
                vehicle.IsDeleted = 1;
                _context.SaveChanges();

                return "vehicle_deleted";
            }
            catch (MySqlException ex)
            {
                // Handle exceptions and log the error
                Logger.SendException("MechApp", "vehicles", "DeleteVehicle", ex);
                return "error";
            }
        }

    }

    public class AddVehicleOb
    {
        public string? Producer { get; set; }
        public string? Model { get; set; }
        public string? ProduceDate { get; set; }
        public int? Mileage { get; set; }
        public string? Vin { get; set; }
        public string? EngineNumber { get; set; }
        public string? RegistrationNumber { get; set; }
        public int? EnginePower { get; set; }
        public string? EngineCapacity { get; set; }
        public int? Owner { get; set; }
    }

    public class EditVehicleOb
    {
        public int? id { get; set; }
        public string? ProduceDate { get; set; }
        public int? Mileage { get; set; }
        public string? EngineNumber { get; set; }
        public string? RegistrationNumber { get; set; }
        public int? EnginePower { get; set; }
        public string? EngineCapacity { get; set; }
    }
}
