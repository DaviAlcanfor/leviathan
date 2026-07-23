.PHONY: setup server cli clean

VENV    := server/.venv
PIP     := $(VENV)/bin/pip
PYTHON  := $(VENV)/bin/python

setup: $(VENV)/.stamp

$(VENV)/.stamp: server/requirements.txt
	python -m venv $(VENV)
	$(PIP) install -r server/requirements.txt
	@touch $@

server: setup
	cd server && $(PYTHON) main.py

cli: setup
	cd server && $(PYTHON) cli.py $(ARGS)

clean:
	rm -rf $(VENV)
