import argparse
import sys

def main():
    parser = argparse.ArgumentParser(prog="napcat-plugin-tmp-bot")
    parser.add_argument("--version", action="store_true")
    args = parser.parse_args()
    if args.version:
        print("napcat-plugin-tmp-bot 1.7.4")
        return 0
    print("请使用 src/index.js 的 Napcat/Koishi 入口")
    return 0

if __name__ == "__main__":
    sys.exit(main())

