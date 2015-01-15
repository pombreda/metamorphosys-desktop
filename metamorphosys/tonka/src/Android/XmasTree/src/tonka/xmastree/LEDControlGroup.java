/*
Copyright (C) 2013-2015 MetaMorph Software, Inc

Permission is hereby granted, free of charge, to any person obtaining a
copy of this data, including any software or models in source or binary
form, as well as any drawings, specifications, and documentation
(collectively "the Data"), to deal in the Data without restriction,
including without limitation the rights to use, copy, modify, merge,
publish, distribute, sublicense, and/or sell copies of the Data, and to
permit persons to whom the Data is furnished to do so, subject to the
following conditions:

The above copyright notice and this permission notice shall be included
in all copies or substantial portions of the Data.

THE DATA IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
THE AUTHORS, SPONSORS, DEVELOPERS, CONTRIBUTORS, OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE DATA OR THE USE OR OTHER DEALINGS IN THE DATA.  
*/

package tonka.xmastree;

import java.util.ArrayList;
import java.util.List;

import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewParent;
import android.widget.RadioButton;
import android.widget.RadioGroup;

public class LEDControlGroup {

    List<RadioButton> radios = new ArrayList<RadioButton>();
    LEDControlListener listener;

    /**
     * Constructor, which allows you to pass number of RadioButton instances,
     * making a group.
     * 
     * @param radios
     *            One RadioButton or more.
     */
    public LEDControlGroup(LEDControlListener listener, RadioButton... radios) {
        super();

        this.listener = listener;
        
        for (RadioButton rb : radios) {
            this.radios.add(rb);
            rb.setOnClickListener(onClick);
        }
    }

    /**
     * Constructor, which allows you to pass number of RadioButtons 
     * represented by resource IDs, making a group.
     * 
     * @param activity
     *            Current View (or Activity) to which those RadioButtons 
     *            belong.
     * @param radiosIDs
     *            One RadioButton or more.
     */
    public LEDControlGroup(View activity, int... radiosIDs) {
        super();

        for (int radioButtonID : radiosIDs) {
            RadioButton rb = (RadioButton)activity.findViewById(radioButtonID);
            if (rb != null) {
                this.radios.add(rb);
                rb.setOnClickListener(onClick);
            }
        }
    }
    
    public void setEnabled(boolean enabled) {
    	for (RadioButton rb : radios) {
    		rb.setEnabled(enabled);
    	}
    }
    
    public String getCurrentTag() {
    	for (RadioButton rb : radios) {
    		if (rb.isChecked()) {
    			return rb.getTag().toString();
    		}
    	}
    	return "";
    }

    /**
     * This occurs everytime when one of RadioButtons is clicked, 
     * and deselects all others in the group.
     */
    OnClickListener onClick = new OnClickListener() {

        @Override
        public void onClick(View v) {

            // let's deselect all radios in group
            for (RadioButton rb : radios) {

                ViewParent p = rb.getParent();
                if (p.getClass().equals(RadioGroup.class)) {
                    // if RadioButton belongs to RadioGroup, 
                    // then deselect all radios in it 
                    RadioGroup rg = (RadioGroup) p;
                    rg.clearCheck();
                } else {
                    // if RadioButton DOES NOT belong to RadioGroup, 
                    // just deselect it
                    rb.setChecked(false);
                }
            }

            // now let's select currently clicked RadioButton
            if (v.getClass().equals(RadioButton.class)) {
                RadioButton rb = (RadioButton) v;
                rb.setChecked(true);
                if (listener != null) {
                	listener.LEDSelected(rb.getTag().toString(), rb);
                }
            }

        }
    };
}