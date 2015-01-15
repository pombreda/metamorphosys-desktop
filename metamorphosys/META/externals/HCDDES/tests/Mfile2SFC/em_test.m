function [results,reference,T,X,Y] = em_test(model, basedir)
%
%[results,reference, T,X,Y] = test_esmol(model, basedir);
%[results,reference, T,X,Y] = test_esmol(model);
%
%   Given a Simulink model containing some number of top-level output ports,
%   compares the results computed between a native Simulink simulation and
%   an execution instance of the actual source code generated by the ESMOL
%   tool chain.
%
%   $model   - name of the simulink model (not the name of the .mdl file) 
%   $basedir - directory in which the above model resides; optional if 
%              named the same as the model and under the current 
%              subdirectory (e.g. ./sl_basic/, containing the sl_basic.mdl 
%              file)
%

%TODO: replace all '\' & '\\' strings with '/'

    %testintermediate = true;
    %makegoldfiles = false;

    script     = which('em_test');
    scriptdir  = script(1:length(script)-9);    
    BIN_DIR    = [scriptdir, '..\..\bin'];        % where are the ESMOL binaries?
    %clc;
    
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%    
    % Create a temporary directory to hold all of our artifacts
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% 
    if ( length(model)>4 && strcmp('.m', model(length(model)-3:length(model))))
        model = model(1:length(model)-2);  % not supposed to contain '.m' 
    end
    if (~exist('basedir')) 
        basedir = model;    % second param is optional, given naming convention
    end    
    pushd      ( toString(basedir) );    
    OUTPUT_DIR = [pwd,'\em_test'];
    outputdir = toString(OUTPUT_DIR);
    %goldendir = sprintf('%sGOLDEN',outputdir);
    clean();   
    dos(['mkdir ', OUTPUT_DIR]);
    cd (  outputdir);
    %if(makegoldfiles)
    %    dos(['mkdir ', goldendir]);
    %end

    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%    
    %% Toolchain stages:  mdl2mga | addtypes | codegen
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    %disp 'Translating to VCP ... '
    %cmdline = sprintf('%s\\mdl2mga ..\\%s.mdl %s.xml', BIN_DIR,model,model);
    %assert_dos(cmdline);
    %cmdline = sprintf('%s\\mdl2mga ..\\%s.mdl %s.mga', BIN_DIR,model,model);
    %assert_dos(cmdline);
    
    %if (makegoldfiles)
    %    cmdline = sprintf('copy %s.xml %s\\%sMDL2MGA.xml',model,goldendir,model);
    %    dos(cmdline);
    %end
    %if (testintermediate)
    %    disp 'Checking MDL2MGA output ... '
    %    cd(scriptdir);
    %    cmdline = sprintf('UdmModelDiff %s\\%s.xml %sGOLDEN\\%sMDL2MGA.xml ECSL_DP_udm.xml', outputdir,model,outputdir,model);
    %    dos(cmdline); %UdmModelDiff sometimes fails on identical files, so don't assert for now
    %    cd(outputdir);
    %end
    
    %disp 'Type inferencing ...'
    %cmdline = sprintf('%s\\addtypes %s.xml', BIN_DIR,model);
    %assert_dos(cmdline);  
    %BUG?: turn this on, once the major bugs are solved
    %cmdline = sprintf('%s\\addtypes %s.mga', BIN_DIR,model);
    %assert_dos(cmdline);  
  
    %if (makegoldfiles)
    %    cmdline = sprintf('copy %s.xml %s\\%sAddTypes.xml',model,goldendir,model);
    %    dos(cmdline);
    %end
    %if (testintermediate)
    %    disp 'Checking AddTypes output ... '
    %    cd(scriptdir);
    %    cmdline = sprintf('UdmModelDiff %s\\%s.xml %sGOLDEN\\%sAddTypes.xml ECSL_DP_udm.xml', outputdir,model,outputdir,model);
    %    dos(cmdline);
    %    cd(outputdir);
    %end
    disp 'Copying .m file into test directory'
    cmdline = sprintf('copy ..\\%s.m %s.m', model, model);
    assert_dos(cmdline);
    
    disp 'Translating to SFC ...'
    cmdline = sprintf('%s\\mfile2SFCtest %s.m', BIN_DIR,model);
    assert_dos(cmdline);  
    %cmdline = sprintf('%s\\sl_codegen %s.xml', BIN_DIR,model);
    %assert_dos(cmdline);   
    %cmdline = sprintf('%s\\sf_codegen %s.xml', BIN_DIR,model);
    %assert_dos(cmdline);
    
    %if (makegoldfiles)
    %    cmdline = sprintf('copy %s_SFC.xml %s\\',model,goldendir);
    %    dos(cmdline);
    %end
    %if (testintermediate)
    %    disp 'Checking SL_CodeGen output ... '
    %    cd(scriptdir);
    %    cmdline = sprintf('UdmModelDiff %s\\%s_SFC.xml %sGOLDEN\\%s_SFC.xml SFC_udm.xml', outputdir,model,outputdir,model);
    %    dos(cmdline);
    %    cd(outputdir);
    %end
    
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%    
    %% Auto-generate main() routine
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%      
    disp 'Generating source code ...'
    %../../bin/ctestgen 'FileType="sl_basic_SFC.xml";"output.xml" !"SFC.xsd"'
    cmdline = sprintf('%s\\ctestgen "FileType=%s_SFC.xml;%s_main.xml"', BIN_DIR,model,model);
    assert_dos(cmdline);  
    cmdline = sprintf('%s\\SFCPrinter -j %s_main.xml', BIN_DIR,model);
    assert_dos(cmdline);
    
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%    
    %% Compile & Execute
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%    
    disp 'Compiling ...'
    jcompile(model);
    
    %disp 'Simulations ...'
    % MATLAB Simulation
    %addpath ..
    %[T,X,Y] = sim(toString(model));   
    %reference = Y;  %WAS: [T,Y];
    %save simulation_matlab Y
    %rmpath  ..
    % ESMOL Simulation
    %!esmol.exe > simulation_esmol.csv
    %results = load('simulation_esmol.csv');
        
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    %% Compare Results
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%    
    %disp 'Verifying results ...'
    %if ( ~sameDimensions(results,reference) ) 
    %    FAILED = true;
    %else    
    %    [rows,cols] = size(reference);
    %    FAILED = ~( sum(sum((results == reference) | (isnan(results) & isnan(reference)))) == rows*cols );
    %end
    %if (FAILED)
    %    disp ' '
    %    disp 'MATLAB:'
    %    disp '============================================================'
    %    reference'
    %    disp 'ESMoL:'
    %    disp '============================================================'        
    %    results'
    %    disp ' '        
    %    disp 'MATLAB-ESMoL:'
    %    disp '============================================================'   
    %    if (size(reference) == size(results))
    %        reference' - results'
    %    else
    %        disp 'simulations returned results of dissimilar dimensions'
    %    end        
    %    disp ' '        
    %    disp 'FAILURE !!!'        
    %else
    %    cd ..
    %    % clean();
    %    disp ' '
    %    disp 'SUCCESS !!!'    
    %end
    popd();
    %assert(~FAILED);    
end


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%% Utility Functions
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
function dos(cmd)  
  eval(sprintf('!%s',cmd));
end
function assert_dos(cmd)
    disp(['    executing: ', cmd ]);
    [err,stdout] = system(cmd);
    if(err)
        disp(['ERROR(',num2str(err),'): `', cmd, '` failed']);
        stdout
        disp ' '
        disp 'FAILURE !!!' 
        popd();
        error(['ERROR(',num2str(err),'): `', cmd, '` failed'])        
    end
end
function del(file)
  cmd = sprintf('del %s', file);
  dos(cmd)
end
function result = toString(var)
  result = sprintf('%s',var);
end
function pushd(dir)
  global ORIG_DIR;
  ORIG_DIR = pwd;
  cd( toString(dir) );
end
function popd() 
  global ORIG_DIR;
  cd( toString(ORIG_DIR) );
end
function result = sameDimensions(X,Y)
    result = sum(size(X)~=size(Y)) == 0;
end


function clean()
    disp ('Cleaning up [previous] temporary files ...');
    % Convoluted way, which avoids some noisy err msgs for the user
    files = dir;
    for i= 1:length(files)
        if (strcmp(files(i).name,'esmol_test'))
            !del /s/q esmol_test
            !rmdir esmol_test
        end
    end
end
function compile()
    % TODO: the SF-code-generator automatically creates unused SFunc
    % wrapper code
    !del sfunc_wrapper_*.c 
    
    CC  = 'mbuild -output esmol';
    SRC = ' *.c';
    global ORIG_DIR;
    INCLUDES = [ ' -I"',ORIG_DIR,'"' ];
    assert_dos([CC, INCLUDES, SRC]);
end
function jcompile(model)
    
    JC  = sprintf('"%%JDK_PATH%%\\bin\\javac" %s_class.java',model);
    disp(JC)
    assert_dos(JC);
end



