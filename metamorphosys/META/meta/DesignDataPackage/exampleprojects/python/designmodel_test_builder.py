# Copyright (C) 2013-2015 MetaMorph Software, Inc

# Permission is hereby granted, free of charge, to any person obtaining a
# copy of this data, including any software or models in source or binary
# form, as well as any drawings, specifications, and documentation
# (collectively "the Data"), to deal in the Data without restriction,
# including without limitation the rights to use, copy, modify, merge,
# publish, distribute, sublicense, and/or sell copies of the Data, and to
# permit persons to whom the Data is furnished to do so, subject to the
# following conditions:

# The above copyright notice and this permission notice shall be included
# in all copies or substantial portions of the Data.

# THE DATA IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
# THE AUTHORS, SPONSORS, DEVELOPERS, CONTRIBUTORS, OR COPYRIGHT HOLDERS BE
# LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
# OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
# WITH THE DATA OR THE USE OR OTHER DEALINGS IN THE DATA.  

# =======================
# This version of the META tools is a fork of an original version produced
# by Vanderbilt University's Institute for Software Integrated Systems (ISIS).
# Their license statement:

# Copyright (C) 2011-2014 Vanderbilt University

# Developed with the sponsorship of the Defense Advanced Research Projects
# Agency (DARPA) and delivered to the U.S. Government with Unlimited Rights
# as defined in DFARS 252.227-7013.

# Permission is hereby granted, free of charge, to any person obtaining a
# copy of this data, including any software or models in source or binary
# form, as well as any drawings, specifications, and documentation
# (collectively "the Data"), to deal in the Data without restriction,
# including without limitation the rights to use, copy, modify, merge,
# publish, distribute, sublicense, and/or sell copies of the Data, and to
# permit persons to whom the Data is furnished to do so, subject to the
# following conditions:

# The above copyright notice and this permission notice shall be included
# in all copies or substantial portions of the Data.

# THE DATA IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
# THE AUTHORS, SPONSORS, DEVELOPERS, CONTRIBUTORS, OR COPYRIGHT HOLDERS BE
# LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
# OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
# WITH THE DATA OR THE USE OR OTHER DEALINGS IN THE DATA.  

import sys
import xml.dom.minidom

sys.path.insert(0, '../../lib/python')
import avm
from avm import modelica, cad


if __name__ == "__main__":
    d = avm.Design()
    d.Name = "TestDesign"
    d.DesignID = "id123-234"


    # Build a top-level container
    cmp_Root = avm.Compound(Name="RootContainer")
    d.RootContainer = cmp_Root
    
    ### Test objects from the iFAB package
    part1 = iFAB.part()
    part2 = iFAB.part()

    part1.id = "16106CF7-2643-46A1-970C-75DB50F6C76B"
    part2.id = "26106CF7-2643-46A1-970C-75DB50F6C76B"

    mechanical1 = iFAB.mechanical()
    mechanical1.fasteningMethod = "Bolted"
    mechanical1.part1 = part1
    mechanical1.part2 = part2

    mechanical1.description = "BaseSeamTest1"
    mechanical1.id = "36106CF7-2643-46A1-970C-75DB50F6C76B"

    cmp_Root.JoinData.append(mechanical1)
    
    ### End of iFAB test 


    # Create a Component Instance
    ci_Component1 = avm.ComponentInstance()
    ci_Component1.ComponentID = "abc-123-4135-23421"
    ci_Component1.Name = "ExampleComponent_LeftSide"
    ci_Component1.ID = "id1"
    cmp_Root.ComponentInstance.append(ci_Component1)

    # Add a couple of ComponentPortInstances to Component1
    cpi_Comp1Port1 = avm.ComponentPortInstance()
    cpi_Comp1Port1.IDinComponentModel = "id0"
    cpi_Comp1Port1.ID = "id11"
    ci_Component1.PortInstance.append(cpi_Comp1Port1)
    cpi_Comp1Port2 = avm.ComponentPortInstance()
    cpi_Comp1Port2.IDinComponentModel = "id1"
    cpi_Comp1Port2.ID = "id12"
    ci_Component1.PortInstance.append(cpi_Comp1Port2)


    # Create a subcontainer
    cmp_Subsys = avm.Compound(Name="Subsystem")
    cmp_Root.Container.append(cmp_Subsys)

    # Create a Component Instance in the subcontainer.
    # It will be a "second instance" of the same component as the first,
    # and has the same ComponentID because they share the same definition.
    ci_Component2 = avm.ComponentInstance(Name="ExampleComponent_InSubsystem",ComponentID="abc-123-4135-23421",ID="id2")
    cmp_Subsys.ComponentInstance.append(ci_Component2)

    cpi_Comp2Port1 = avm.ComponentPortInstance(IDinComponentModel="id0",ID="id13")
    ci_Component2.PortInstance.append(cpi_Comp2Port1)
    cpi_Comp2Port2 = avm.ComponentPortInstance(IDinComponentModel="id1",ID="id14")
    ci_Component2.PortInstance.append(cpi_Comp2Port2)

    # Create a port on the subcontainer to pass-through a connection.
    p_SubsysPort1 = modelica.Connector(Name="PassThroughPort",Class="Modelica.Mechanics.MultiBody.Interfaces.Frame",ID="id15")
    cmp_Subsys.Port.append(p_SubsysPort1)

    # Map some ports
    cpi_Comp1Port1.PortMap = []
    cpi_Comp1Port1.PortMap.append(p_SubsysPort1.ID)
    cpi_Comp2Port1.PortMap = []
    cpi_Comp2Port1.PortMap.append(p_SubsysPort1.ID)

    ### Use minidom to pretty-print the xml
    dom_d = xml.dom.minidom.parseString(d.toxml("utf-8"))
    xml_d = dom_d.toprettyxml()
    print xml_d