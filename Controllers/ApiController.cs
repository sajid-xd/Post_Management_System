using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace mycourier.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly string _conn;

        public ApiController(IConfiguration config)
        {
            _conn = config.GetConnectionString("mycourierContext");
        }

        // GET: api/api
        [HttpGet]
        public IActionResult GetAll()
        {
            var records = new List<ShipmentRecord>();

            const string sql = "SELECT * FROM deliveries";
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql, con);
            con.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                records.Add(MapShipment(reader));
            }
            return Ok(records);
        }

        // GET: api/api/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            const string sql = "SELECT * FROM deliveries WHERE id=@id";
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return Ok(MapShipment(reader));
            }
            return NotFound();
        }

        // POST: api/api
        [HttpPost]
        public IActionResult Create([FromBody] ShipmentInput s)
        {
            const string sql = @"
                INSERT INTO deliveries
                (from_address,to_address,sender_name,receiver_name,
                 sender_id,agent_id,service_id,weight_id,location_id,
                 tracking_id,status,created_at)
                VALUES
                (@from,@to,@sname,@rname,@sid,@aid,@ser,@wid,@lid,@track,@status,GETDATE());
                SELECT SCOPE_IDENTITY();";

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@from", s.From_Address);
            cmd.Parameters.AddWithValue("@to", s.To_Address);
            cmd.Parameters.AddWithValue("@sname", s.Sender_Name);
            cmd.Parameters.AddWithValue("@rname", s.Receiver_Name);
            cmd.Parameters.AddWithValue("@sid", s.Sender_Id);
            cmd.Parameters.AddWithValue("@aid", s.Agent_Id);
            cmd.Parameters.AddWithValue("@ser", s.Service_Id);
            cmd.Parameters.AddWithValue("@wid", s.Weight_Id);
            cmd.Parameters.AddWithValue("@lid", s.Location_Id);
            cmd.Parameters.AddWithValue("@track", s.Tracking_Id);
            cmd.Parameters.AddWithValue("@status", s.Status ?? "Pending");
            con.Open();
            var newId = Convert.ToInt32(cmd.ExecuteScalar());
            return Ok(new { id = newId });
        }

        // PUT: api/api/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ShipmentInput s)
        {
            const string sql = @"
                UPDATE deliveries SET
                  from_address=@from, to_address=@to, sender_name=@sname,
                  receiver_name=@rname, sender_id=@sid, agent_id=@aid,
                  service_id=@ser, weight_id=@wid, location_id=@lid,
                  tracking_id=@track, status=@status
                WHERE id=@id";

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@from", s.From_Address);
            cmd.Parameters.AddWithValue("@to", s.To_Address);
            cmd.Parameters.AddWithValue("@sname", s.Sender_Name);
            cmd.Parameters.AddWithValue("@rname", s.Receiver_Name);
            cmd.Parameters.AddWithValue("@sid", s.Sender_Id);
            cmd.Parameters.AddWithValue("@aid", s.Agent_Id);
            cmd.Parameters.AddWithValue("@ser", s.Service_Id);
            cmd.Parameters.AddWithValue("@wid", s.Weight_Id);
            cmd.Parameters.AddWithValue("@lid", s.Location_Id);
            cmd.Parameters.AddWithValue("@track", s.Tracking_Id);
            cmd.Parameters.AddWithValue("@status", s.Status ?? "Pending");
            con.Open();
            int rows = cmd.ExecuteNonQuery();
            return rows > 0 ? Ok("Updated") : NotFound();
        }

        // DELETE: api/api/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            const string sql = "DELETE FROM deliveries WHERE id=@id";
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            int rows = cmd.ExecuteNonQuery();
            return rows > 0 ? Ok("Deleted") : NotFound();
        }

        // Helper: Map a data record to ShipmentRecord object
        private static ShipmentRecord MapShipment(SqlDataReader reader)
        {
            return new ShipmentRecord
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                From_Address = reader["from_address"].ToString(),
                To_Address = reader["to_address"].ToString(),
                Sender_Name = reader["sender_name"].ToString(),
                Receiver_Name = reader["receiver_name"].ToString(),
                Sender_Id = reader.GetInt32(reader.GetOrdinal("sender_id")),
                Agent_Id = reader.GetInt32(reader.GetOrdinal("agent_id")),
                Service_Id = reader.GetInt32(reader.GetOrdinal("service_id")),
                Weight_Id = reader.GetInt32(reader.GetOrdinal("weight_id")),
                Location_Id = reader.GetInt32(reader.GetOrdinal("location_id")),
                Tracking_Id = reader["tracking_id"].ToString(),
                Status = reader["status"].ToString(),
                Created_At = reader.GetDateTime(reader.GetOrdinal("created_at"))
            };
        }
    }

    // DTO for POST/PUT
    public class ShipmentInput
    {
        public string From_Address { get; set; }
        public string To_Address { get; set; }
        public string Sender_Name { get; set; }
        public string Receiver_Name { get; set; }
        public int Sender_Id { get; set; }
        public int Agent_Id { get; set; }
        public int Service_Id { get; set; }
        public int Weight_Id { get; set; }
        public int Location_Id { get; set; }
        public string Tracking_Id { get; set; }
        public string Status { get; set; }
    }

    // Strongly typed model matching the `deliveries` table
    public class ShipmentRecord
    {
        public int Id { get; set; }
        public string From_Address { get; set; }
        public string To_Address { get; set; }
        public string Sender_Name { get; set; }
        public string Receiver_Name { get; set; }
        public int Sender_Id { get; set; }
        public int Agent_Id { get; set; }
        public int Service_Id { get; set; }
        public int Weight_Id { get; set; }
        public int Location_Id { get; set; }
        public string Tracking_Id { get; set; }
        public string Status { get; set; }
        public DateTime Created_At { get; set; }
    }
}
