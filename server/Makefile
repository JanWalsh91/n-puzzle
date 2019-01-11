NAME = run
SRCS := Server

EXT  :=.cs
SRCS := $(addsuffix $(EXT), $(SRCS))

SRCS_DIR := ./src
SRCS := $(addprefix $(SRCS_DIR)/, $(SRCS))

CC := mcs

all: $(NAME)

$(NAME): $(SRCS)
	$(CC) $(SRCS) -out:$(NAME)

fclean:
		rm -f $(NAME)

re: fclean all

PHONY: re all fclean

.PHONY: all fclean re
