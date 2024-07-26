##
# This file is part of the Metasploit Framework and may be subject to
# redistribution and commercial restrictions. Please see the Metasploit
# web site for more information on licensing and terms of use.
#   http://metasploit.com/
##

require 'msf/core'
require 'rex'

class Metasploit3 < Msf::Post

	def initialize(info={})
		super( update_info( info,
			'Name'          => 'Windows Gather Domain User Sessions',
			'Description'   => %q{
				This module enumerates active domain user sessions.
			},
			'License'       => MSF_LICENSE,
			'Author'        => [ 'Scott Sutherland <scott.sutherland[at]nullbind.com>'],
			'Platform'      => [ 'windows' ],
			'SessionTypes'  => [ 'meterpreter' ]
		))
		
		register_options(
			[
				OptString.new('DOMAIN',  [false, 'Domain to target, default to computer\'s domain', '']),
				OptString.new('TYPE',  [true, 'Search type: GROUPS or USERS', 'GROUPS']),
				OptString.new('GROUP',  [false, 'Domain groups to search for.', 'Domain Admins, Forrest Admins, Enterprise Admins']),
				OptString.new('USER',  [false, 'Domain users to search for.', '']),
				OptBool.new('LOOP',  [false, 'Scan for sessions continuously', 'false']),
			], self.class
		)
		
	end

	def run
	
		#Create an array to hold the list of domains
		#Create an array to hold the domain controller IP addresses
		#Create an array to hold the session information login,domain,ip,idle time,session time
		#Create an array to hold the group information login,domain
		#Create an array to hold final list domain, group, user, ip
	
		#Get current domain or set it from the option
		#Check if domain == computername, if so fail
	
		#Get a list of all of the domains in the forrest
		# adfind -sc domainlist 
		
		#Get a list of trust for the current domain
		# adfind -sc trustdmp
		
		#Get a list of the domain controllers for the current domain
		# adfind -sc dclist
		# add to the domain controllers array
		
		#Get a list of the domain controllers for the trusted domains
		# adfind -b dc=trusted,dc=otherdomain,dc=domainname,dc=com -sc
		# add to the domain controllers array
		
		#For each domain controller grab the active sessions add to array 
		#note: most of this code is based on mubix's enum_domains module
		
		buffersize = 1000
		#getsize = client.railgun.netapi32.NetSessionEnum(nil,nil,nil,10,4,buffersize,4,4,nil)
		#buffersize = getsize['bufptr']

		result = client.railgun.netapi32.NetSessionEnum(nil,nil,nil,10,4,buffersize,4,4,nil)
		
		count = result['totalentries']
		print_status("#{count} Sessions found.")
		startmem = result['bufptr']

		base = 0
		mysessions = []
		mem = client.railgun.memread(startmem, 8*count) #note: this dies if count= 0; at handling; http://msdn.microsoft.com/en-us/library/windows/desktop/bb525382(v=vs.85).aspx
		count.times{|i|
			x = {}
			
			# Grab returned
			client_ptr = mem[(base + 0),4].unpack("V*")[0]
			username_ptr = mem[(base + 4),4].unpack("V*")[0]
			
			# Parse returned data
			x[:client] = client.railgun.memread(client_ptr,255).split("\0\0")[0].split("\0").join	
			x[:username] = client.railgun.memread(username_ptr,255).split("\0\0")[0].split("\0").join	
			
			#Print session - only getting 2nd column
			print_status("client, username, active time, idle time")
			print_status("#{x[:client]}, #{x[:username]}")
			
			mysessions << x
			base = base + 8
		}		
		
	end
end
