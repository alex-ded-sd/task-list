import { DataSource } from "@angular/cdk/collections";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { map } from "rxjs/operators";
import { Observable, of as observableOf, merge } from "rxjs";

// TODO: Replace this with your own data model type
export interface TaskListItem {
	name: string;
	id: number;
	canRaise: boolean;
	canReduce: boolean;
}

// TODO: replace this with real data from your application
const EXAMPLE_DATA: TaskListItem[] = [
	{ id: 1, name: "Hydrogen", canRaise: false, canReduce: true },
	{ id: 2, name: "Helium", canRaise: true, canReduce: true },
	{ id: 3, name: "Lithium", canRaise: true, canReduce: true },
	{ id: 4, name: "Beryllium", canRaise: true, canReduce: true },
	{ id: 5, name: "Boron", canRaise: true, canReduce: true },
	{ id: 6, name: "Carbon", canRaise: true, canReduce: true },
	{ id: 7, name: "Nitrogen", canRaise: true, canReduce: true },
	{ id: 8, name: "Oxygen", canRaise: true, canReduce: true },
	{ id: 9, name: "Fluorine", canRaise: true, canReduce: true },
	{ id: 10, name: "Neon", canRaise: true, canReduce: true },
	{ id: 11, name: "Sodium", canRaise: true, canReduce: true },
	{ id: 12, name: "Magnesium", canRaise: true, canReduce: true },
	{ id: 13, name: "Aluminum", canRaise: true, canReduce: true },
	{ id: 14, name: "Silicon", canRaise: true, canReduce: true },
	{ id: 15, name: "Phosphorus", canRaise: true, canReduce: true },
	{ id: 16, name: "Sulfur", canRaise: true, canReduce: true },
	{ id: 17, name: "Chlorine", canRaise: true, canReduce: true },
	{ id: 18, name: "Argon", canRaise: true, canReduce: true },
	{ id: 19, name: "Potassium", canRaise: true, canReduce: true },
	{ id: 20, name: "Calcium", canRaise: true, canReduce: false }
];

/**
 * Data source for the TaskList view. This class should
 * encapsulate all logic for fetching and manipulating the displayed data
 * (including sorting, pagination, and filtering).
 */
export class TaskListDataSource extends DataSource<TaskListItem> {
	data: TaskListItem[] = EXAMPLE_DATA;
	paginator: MatPaginator;
	sort: MatSort;

	constructor() {
		super();
	}

	/**
	 * Connect this data source to the table. The table will only update when
	 * the returned stream emits new items.
	 * @returns A stream of the items to be rendered.
	 */
	connect(): Observable<TaskListItem[]> {
		// Combine everything that affects the rendered data into one update
		// stream for the data-table to consume.
		const dataMutations = [observableOf(this.data), this.paginator.page, this.sort.sortChange];

		return merge(...dataMutations).pipe(
			map(() => {
				return this.getPagedData(this.getSortedData([...this.data]));
			})
		);
	}

	/**
	 *  Called when the table is being destroyed. Use this function, to clean up
	 * any open connections or free any held resources that were set up during connect.
	 */
	disconnect() {}

	/**
	 * Paginate the data (client-side). If you're using server-side pagination,
	 * this would be replaced by requesting the appropriate data from the server.
	 */
	private getPagedData(data: TaskListItem[]) {
		const startIndex = this.paginator.pageIndex * this.paginator.pageSize;
		return data.splice(startIndex, this.paginator.pageSize);
	}

	/**
	 * Sort the data (client-side). If you're using server-side sorting,
	 * this would be replaced by requesting the appropriate data from the server.
	 */
	private getSortedData(data: TaskListItem[]) {
		if (!this.sort.active || this.sort.direction === "") {
			return data;
		}

		return data.sort((a, b) => {
			const isAsc = this.sort.direction === "asc";
			switch (this.sort.active) {
				case "name":
					return compare(a.name, b.name, isAsc);
				case "id":
					return compare(+a.id, +b.id, isAsc);
				default:
					return 0;
			}
		});
	}
}

/** Simple sort comparator for example ID/Name columns (for client-side sorting). */
function compare(a, b, isAsc) {
	return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
}
