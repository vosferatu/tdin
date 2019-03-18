MCS=mcs
DLL=-t:library
DLL_DIR=./libs

GUI_DIR=$(SRC_DIR)/gui
GUI_SRC=$(wildcard $(GUI_DIR)/*cs) $(wildcard $(GUI_DIR)/*/*.cs)
GUI_DLL=$(DLL_DIR)/gui.dll
GUI_LIBS=-pkg:glade-sharp-2.0

PROD_DIR=$(SRC_DIR)/products
PROD_SRC=$(wildcard $(PROD_DIR)/*.cs)
PROD_DLL=$(DLL_DIR)/products.dll

SRC_DIR=./src
SRC_SRC=$(wildcard $(SRC_DIR)/*.cs)
SRC_PKGS=-pkg:glade-sharp-2.0
SRC_LIBS=-r:$(GUI_DLL) -r:$(PROD_DLL)
SRC_EXE=./app.exe

all: $(SRC_EXE)

gui: $(GUI_DLL)

products: $(PROD_DLL)

$(GUI_DLL): $(GUI_SRC)
	@$(MCS) $(DLL) $(GUI_LIBS) $^ -out:$@
	@echo "GUI compiled!"

$(PROD_DLL): $(PROD_SRC)
	@$(MCS) $(DLL) $^ -out:$@
	@echo "Products compiled!"

$(SRC_EXE): $(GUI_DLL) $(PROD_DLL) $(SRC_SRC)
	@$(MCS) $(SRC_SRC) $(SRC_LIBS) $(SRC_PKGS) -out:$@
	@echo "App compiled!"

clean:
	@rm -f $(GUI_DLL) $(PROD_DLL) $(SRC_EXE)
