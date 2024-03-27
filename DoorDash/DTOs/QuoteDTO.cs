namespace DoorDash.DTOs
{
    using System;
    using System.Collections.Generic;


    public class QuoteDTO
    {
        public string external_delivery_id { get; set; }
        public string locale { get; set; }
        public string order_fulfillment_method { get; set; }
        public string origin_facility_id { get; set; }
        public string pickup_address { get; set; }
        public string pickup_business_name { get; set; }
        public string pickup_phone_number { get; set; }
        public string pickup_instructions { get; set; }
        public string pickup_reference_tag { get; set; }
        public string pickup_external_business_id { get; set; }
        public string pickup_external_store_id { get; set; }
        public PickupVerificationMetadataDTO pickup_verification_metadata { get; set; }
        public string dropoff_address { get; set; }
        public string dropoff_business_name { get; set; }
        public DropoffLocationDTO dropoff_location { get; set; }
        public string dropoff_phone_number { get; set; }
        public string dropoff_instructions { get; set; }
        public string dropoff_contact_given_name { get; set; }
        public string dropoff_contact_family_name { get; set; }
        public bool dropoff_contact_send_notifications { get; set; }
        public DropoffOptionsDTO dropoff_options { get; set; }
        public ShoppingOptionsDTO shopping_options { get; set; }
        public int order_value { get; set; }
        public List<ItemDTO> items { get; set; }
        public DateTime pickup_time { get; set; }
        public DateTime dropoff_time { get; set; }
        public TimeWindowDTO pickup_window { get; set; }
        public TimeWindowDTO dropoff_window { get; set; }
        public bool contactless_dropoff { get; set; }
        public string action_if_undeliverable { get; set; }
        public int tip { get; set; }
        public OrderContainsDTO order_contains { get; set; }
        public List<string> dasher_allowed_vehicles { get; set; }
        public bool dropoff_requires_signature { get; set; }
        public string promotion_id { get; set; }
        public int dropoff_cash_on_delivery { get; set; }
    }

    public class PickupVerificationMetadataDTO
    {
        public string verification_type { get; set; }
        public string verification_code { get; set; }
        public string verification_format { get; set; }
    }

    public class DropoffLocationDTO
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class DropoffOptionsDTO
    {
        public string signature { get; set; }
        public string id_verification { get; set; }
        public string proof_of_delivery { get; set; }
        public string catering_setup { get; set; }
    }

    public class ShoppingOptionsDTO
    {
        public string payment_method { get; set; }
        public string payment_barcode { get; set; }
        public List<string> payment_gift_cards { get; set; }
        public DateTime ready_for_pickup_by { get; set; }
        public string dropoff_contact_loyalty_number { get; set; }
    }

    public class ItemDTO
    {
        public string name { get; set; }
        public string description { get; set; }
        public int quantity { get; set; }
        public string external_id { get; set; }
        public int external_instance_id { get; set; }
        public double volume { get; set; }
        public double weight { get; set; }
        public double length { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public int price { get; set; }
        public long barcode { get; set; }
        public ItemOptionsDTO item_options { get; set; }
    }

    public class ItemOptionsDTO
    {
        public List<string> substitute_item_ids { get; set; }
        public string weight_unit { get; set; }
        public string substitution_preference { get; set; }
    }

    public class TimeWindowDTO
    {
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
    }

    public class OrderContainsDTO
    {
        public bool alcohol { get; set; }
        public bool pharmacy_items { get; set; }
        public bool age_restricted_pharmacy_items { get; set; }
    }
}


