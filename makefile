MCS=mcs
DLL=-t:library

GUI_DIR=./gui
GUI_SRC=$(wildcard $(GUI_DIR)/*cs)
GUI_DLL=$(GUI_DIR)/gui.dll
GUI_LIBS=-pkg:glade-sharp-2.0

PROD_DIR=./products
PROD_SRC=$(wildcard $(PROD_DIR)/*.cs)
PROD_DLL=$(PROD_DIR)/products.dll

.PHONY: all

clean:
	@rm -f $(GUI_DLL) $(PROD_DLL)

all: gui products

gui: GUI_DLL

products: PROD_DLL

GUI_DLL: 
	@$(MCS) $(DLL) $(GUI_LIBS) $(GUI_SRC) -out:$(GUI_DLL)
	@echo "GUI compiled!"

PROD_DLL:
	@$(MCS) $(DLL) $(PROD_SRC) -out:$(PROD_DLL)
	@echo "Products compiled!"

