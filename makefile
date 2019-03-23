MCS=mcs
DLL=-t:library
DLL_DIR=./libs
SRC_DIR=./src

COMMON_DIR=$(SRC_DIR)/commons
COMMON_SRC=$(wildcard $(COMMON_DIR)/*.cs)
COMMON_DLL=$(DLL_DIR)/common.dll

GUI_DIR=$(SRC_DIR)/gui
GUI_SRC=$(wildcard $(GUI_DIR)/*cs) $(wildcard $(GUI_DIR)/*/*.cs) 
GUI_DLL=$(DLL_DIR)/gui.dll
GUI_LIBS=-pkg:glade-sharp-2.0 -r:$(COMMON_DLL)

PROD_DIR=$(SRC_DIR)/products
PROD_SRC=$(wildcard $(PROD_DIR)/*.cs)
PROD_DLL=$(DLL_DIR)/products.dll
PROD_LIBS=-r:$(COMMON_DLL)

CONTROLLER_DIR=$(SRC_DIR)/controllers
CONTROLLER_SRC=$(wildcard $(CONTROLLER_DIR)/*cs) $(wildcard $(CONTROLLER_DIR)/*/*.cs) 
CONTROLLER_DLL=$(DLL_DIR)/controller.dll
CONTROLLER_LIBS=-r:$(GUI_DLL) -r:$(PROD_DLL) -r:$(COMMON_DLL) -r:System.Runtime.Remoting.dll -pkg:glade-sharp-2.0

SRC_SRC=$(wildcard $(SRC_DIR)/*.cs) 
SRC_PKGS=-pkg:glade-sharp-2.0
SRC_LIBS=-r:$(GUI_DLL) -r:$(PROD_DLL) -r:$(CONTROLLER_DLL) -r:$(COMMON_DLL)
SRC_EXE=./app.exe

all: $(SRC_EXE)

$(COMMON_DLL): $(COMMON_SRC)
	@mkdir -p libs
	@$(MCS) $(DLL) $^ -out:$@ 

$(CONTROLLER_DLL): $(CONTROLLER_SRC)
	@mkdir -p libs
	@$(MCS) $(DLL) $(CONTROLLER_LIBS) $^ -out:$@
	@echo " --> Controllers compiled!"

$(GUI_DLL): $(GUI_SRC)
	@mkdir -p libs
	@$(MCS) $(DLL) $(GUI_LIBS) $^ -out:$@
	@echo " --> GUI compiled!"

$(PROD_DLL): $(PROD_SRC)
	@mkdir -p libs
	@$(MCS) $(DLL) $(PROD_LIBS) $^ -out:$@
	@echo " --> Products compiled!"

$(SRC_EXE): $(COMMON_DLL) $(GUI_DLL) $(PROD_DLL) $(CONTROLLER_DLL) $(SRC_SRC) 
	@mkdir -p libs
	@$(MCS) $(SRC_SRC) $(SRC_LIBS) $(SRC_PKGS) -out:$@
	@echo " --> App compiled!"

clean:
	@rm -f $(GUI_DLL) $(CONTROLLER_DLL) $(PROD_DLL) $(SRC_EXE)
